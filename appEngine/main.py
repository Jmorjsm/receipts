from google.appengine.ext import ndb

import google

import webapp2
import webapp2_extras.json as json

class CardDetails(webapp2.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'application/json'

    # get parameters from request
    data = {
      "trailingDigits": self.request.get("trailingDigits"),
      "leadingDigits": self.request.get("leadingDigits"),
      "cardType": self.request.get("cardType"),
      "startDate": self.request.get("startDate"),
      "expiryDate": self.request.get("expiryDate")
    }
    if(data["trailingDigits"] == ""):
      # trailingDigits is mandatory
      webapp2.abort(400)
    cardQuery = PartialCard.query(PartialCard.trailingDigits==data["trailingDigits"]).fetch()
    cardMatches = [[card, 0] for card in cardQuery] # (the matched card, number of matching attributes)
    
    for i, card in enumerate(cardMatches):
      # gives each card a score
      # if a provided detail does not match, set score to -1 to discard and proceed to next card
      if(data["leadingDigits"] != "" and card[0].leadingDigits!=""):
        if(card[0].leadingDigits!=data["leadingDigits"]):
          card[1] = -1
          next
        else:
          card[1] += 1
      if(data["cardType"] != "" and card[0].cardType!=""):
        if(card[0].cardType!=data["cardType"]):
          card[1] = -1
          next
        else:
          card[1] += 1
      if(data["startDate"] != "" and card[0].startDate!=""):
        if(card[0].startDate!=data["startDate"]):
          card[1] = -1
          next
        else:
          card[1] += 1
      if(data["expiryDate"] != "" and card[0].expiryDate!=""):
        if(card[0].expiryDate!=data["expiryDate"]):
          card[1] = -1
          next
        else:
          card[1] += 1

    # sort cardMatches by number of matching attributes
    cardMatches.sort(key=lambda card: card[1])
    # remove those with score of -1 (unmatching detail)
    cardMatches = [card[0] for card in cardMatches if card[1] > -1]
    
    # start off with empty array so result can still be returned if no matches
    suggestedEmails = []
    
    for i in range(len(cardMatches)): 
      # could change this to return all customer details depending on client-side implementation
      suggestedEmails.extend([match.customer.email for match in Match.query(Match.card==cardMatches[i]).fetch()])
    
    # json encode the response
    self.response.write(json.encode({'suggestedEmails': suggestedEmails}))

  def put(self):
    # reading parameters sent as JSON
    jsonData = json.decode(self.request.body)
    
    data = {}
    
    for key in ("trailingDigits","leadingDigits","cardType","startDate","expiryDate", "email"):
      # set unspecified attributes to an empty string so they can still be added to the datastore
      if(key not in jsonData):
        if(key == "trailingDigits"):
          # trailingDigits is mandatory
          webapp2.abort(400)
        data[key] = ""
      else:
        data[key] = jsonData[key]

    # create new Card entity
    partialCard = PartialCard(trailingDigits=data["trailingDigits"], leadingDigits=data["leadingDigits"], 
                  cardType=data["cardType"], startDate=data["startDate"], expiryDate=data["expiryDate"])
    
    # Checking if emails have already been saved
    customers = Customer.query(Customer.email==data["email"]).fetch()
    if len(customers) > 0:
      # If a customer with this email already exists, add the new card as a match to their email
      customer = customers[0]
    else:
      customer = Customer(email=data["email"])
      customer.put()

    matches = Match.query((Match.card==partialCard) and (Match.customer==customer)).fetch()

    if(len(matches) == 0):
      match = Match(card=partialCard, customer=customer)

      partialCard.put()
    
      match.put()


class Customer(ndb.Model):
  email = ndb.StringProperty()
  def __str__(self):
    return "Customer: {email: "+ self.email + "}"
  

class PartialCard(ndb.Model):
  trailingDigits = ndb.StringProperty()
  leadingDigits = ndb.StringProperty()
  cardType = ndb.StringProperty()
  startDate = ndb.StringProperty()
  expiryDate = ndb.StringProperty()
  def __str__(self):
    return "PartialCard: {trailingDigits: " + str(self.trailingDigits) + ", leadingDigits: " + str(self.leadingDigits) + ", cardType: " + self.cardType + ", startDate: " + self.startDate + ", expiryDate: " + self.expiryDate + "}"
  
    
class Match(ndb.Model):
  card = ndb.StructuredProperty(PartialCard)
  customer = ndb.StructuredProperty(Customer)
  


app = webapp2.WSGIApplication([
    ('/cardDetails', CardDetails),
], debug=True)
