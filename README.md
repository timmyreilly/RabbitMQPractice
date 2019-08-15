
# Starting Docker: 

`docker run -d --hostname my-rabbit --name some-rabbit rabbitmq:3 -p 5672:5672` 

# Starting Container instance in Azure: 

`az group create --name RabbitMQDemo --location westus`

`az container create --resource-group RabbitMQDemo --name rabbitInstance --image rabbitmq:3 --ports 5672:5672 --dns-name-label rabbitDemo`

Get the logs of the container: 

`az container logs -g RabbitMQDemo --name rabbitinstance`


## Some other examples: 

`az container create --resource-group RabbitMQDemo --name rabbittwo --image rabbitmq:3-management --ports 5672 5673 15672 --dns-name-label rabbittwo --restart-policy OnFailure`

`az container attach -g RabbitMQDemo --name rabbittwo`



# How about creating a cluster in Azure!

Create a resource group: 

`az group create -n groupSandbox --location westus` 

Create an availability set: 

`az vm availability-set create -n RabbitMQAvailabilitySet -g groupSandbox`

Create an Ubuntu VM in said Availability Set: 

`az vm create -g groupSandbox -n rabbitNodeOne --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --vnet-name GroupVnet --subnet rabbitMQSubnet`

`az vm create -g groupSandbox -n rabbitNodeTwo --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --vnet-name GroupVnet --subnet rabbitMQSubnet`

Open our RabbitMQ Ports: 

`az vm open-port -g groupSandbox -n rabbitNodeOne --port 15672 --priority 350`

`az vm open-port -g groupSandbox -n rabbitNodeOne --port 5672 --priority 300`

`az vm open-port -g groupSandbox -n rabbitNodeTwo --port 15672 --priority 350`

`az vm open-port -g groupSandbox -n rabbitNodeTwo --port 5672 --priority 300`

`az vm open-port -g groupSandbox -n rabbitNodeOne --port 4369 --priority 400`

`az vm open-port -g groupSandbox -n rabbitNodeTwo --port 4369 --priority 400`

Get the public IPs of our VMs; 

`az vm list-ip-addresses -g groupSandbox -o table` 

Connect to our machines and install RabbitMQ

`ssh conductor@40.118.230.64`

`ssh conductor@13.64.99.73`

Install RabbitMQ: 

`sudo add-apt-repository 'deb http://www.rabbitmq.com/debian/ testing main`

`sudo apt-get update`

`sudo apt-get -q -y --force-yes install rabbitmq-server`

Create cookie in both machines: 

`echo 'ERLANGCOOKIEVALUE' | sudo tee /var/lib/rabbitmq/.erlang.cookie`
`sudo chown rabbitmq:rabbitmq /var/lib/rabbitmq/.erlang.cookie`
`sudo chmod 400 /var/lib/rabbitmq/.erlang.cookie`
`sudo invoke-rc.d rabbitmq-server start`

Install Management portal for RabbitMQ 

`sudo rabbitmq-plugins enable rabbitmq_management`
`sudo invoke-rc.d rabbitmq-server stop`
`sudo invoke-rc.d rabbitmq-server start`

### [Create an admin user](https://stackoverflow.com/questions/40436425/how-do-i-create-or-add-a-user-to-rabbitmq): 

`sudo rabbitmqctl add_user test test`
`sudo rabbitmqctl set_user_tags  test administrator`
`sudo rabbitmqctl set_permissions -p / test '.*' '.*' '.*'`

### Configure the cluster: 

In rabbitNodeTwo: 

`sudo rabbitmqctl stop_app`
`sudo rabbitmqctl join_cluster rabbit@rabbitNodeOne`
`sudo rabbitmqctl start_app`
`sudo rabbitmqctl set_cluster_name RabbitCluster`

### Login to the management portal: 

In the browser go to: 
`http://13.64.99.73:15672/` || `http://your.vm.ip.address:15672` 

- Username: test
- Password: test 

You should see two nodes! 


# TODO: Load balancing configuration! 

### Create a public ip for our cluster: 

`az network public-ip create -g groupSandbox --name publicIPforRabbit`

Create a load balancer 

`az network lb create -g groupSandbox -n rabbitClusterLoadBalancer --vnet-name GroupVnet --subnet rabbitMQSubnet --sku Basic --backend-pool-name myBackendPool --frontend-ip-name publicIPforRabbit`

Create a health probe: 

`az network lb probe create -g groupSandbox --lb-name rabbitClusterLoadBalancer --name rabbitHealthProbe --protocol tcp --port 5672`

Create a backend pool to our availability set: 

`az network lb rule create -g groupSandbox --lb-name rabbitClusterLoadBalancer --name myRabbitRule --protocol tcp --frontend-port 80 --backend-port 80 --frontend-ip-name myFrontEndIp`


References: 
[Docs.Microsoft.com](https://docs.microsoft.com/en-us/)
[RabbitMQ High-availability clusters on Azure VM by Moim Hossain](https://moimhossain.com/2015/01/23/rabbitmq-high-availability-clusters-on-azure-vm/)