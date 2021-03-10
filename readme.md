##Payment Gateway
This is a simple payment gateway that converses with a Bank then returns the result
Success and failure are published on an event bus, currently using RabbitMq.
There is an event listener ran as a hosted service that will pick up the messages and store them for inspection. There is some sanitation of the message being stored.

###Setup
App uses MongoDb and RabbitMq can setup as following

> docker pull mongo
> docker run --name payment-gateway-db -p 57017:27017 -d mongo
> docker pull rabbitmq
> docker run -d --hostname payment-gateway-rabbit --name payment-gateway-rabbit -p 55672:5672 rabbitmq:3

### Banks
Have set it up so banks are generated by config, currently only the test Bank is setup, but I am imagining that there will be a couple of protocols to implement with perhaps some different configs.
I would create a facade for each protocol that implements IBank then have it built in the factory.

### Api Spec
API
OpenApi Spec can be found here ./openApi.json or /swagger/index.html

RequestPayment
POST: /v1/Payment
Submits payment request and returns success or failure with a tracking Id

GetPaymentRequest
GET: /v1/Payment/{TrackingId}
Gets details of payment request