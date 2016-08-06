# MessagingExample

Example app how to and what kind of messages being send between actors. 

Using:
- Rabbit mq as message broker
- Masstransit as messaging library
- Saga example

## Docker

Consider using docker to install rabbitmq in a container as a light weight vm.
Check out the docker hub page [here](https://hub.docker.com/_/rabbitmq).

Here's how to install rabbit mq with the management plugin:
> docker pull rabbitmq:3-management

Run the container:
> docker run -d -t -i --name rmq -p 8080:15672 -p 5672:5672 rabbitmq:3-management

Here you configure 2 ports, one for the management plugin website and the other for actual message broker instance.
The management plugin UI will be available on port 8080 from the outside.
Ex: http://dockerhost:8080

The connectionstring to rabbitmq will then look like this:
Ex: rabbitmq://dockerhost/randomname/
