
# Starting Docker: 

`docker run -d --hostname my-rabbit --name some-rabbit rabbitmq:3 -p 5672:5672` 

# Starting Container instance in Azure: 

`az group create --name RabbitMQDemo --location westus`

`az container create --resource-group RabbitMQDemo --name rabbitInstance --image rabbitmq:3 --ports 5672:5672 --dns-name-label rabbitDemo`

Get the logs of the container: 

`az container logs -g RabbitMQDemo --name rabbitinstance`


# Some other examples: 

`az container create --resource-group RabbitMQDemo --name rabbittwo --image rabbitmq:3-management --ports 5672 5673 15672 --dns-name-label rabbittwo --restart-policy OnFailure`

`az container attach -g RabbitMQDemo --name rabbittwo`

