docker pull mongo
docker run --name payment-gateway-db -p 57017:27017 -d mongo
docker pull rabbitmq
docker run -d --hostname payment-gateway-rabbit --name payment-gateway-rabbit -p 55672:5672 rabbitmq:3