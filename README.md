
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

`az group create -n sandboxGroup --location westus` 

Create VNET: 

`az network vnet create -n sandboxVnet -g sandboxGroup --subnet-name firstSubnet`

Create Public IP: 

`az network public-ip create -n publicIpForRabbitCluster -g sandboxGroup`

Create a load balancer 

`az network lb create -g sandboxGroup -n rabbitClusterLoadBalancer --public-ip-address publicIpForRabbitCluster --sku Basic --backend-pool-name myBackendPool --frontend-ip-name publicIPforRabbit`

Create a probe / health check? 

`az network lb probe create -g sandboxGroup -n rabbitMQProbe --port 15672 --protocol tcp --lb-name rabbitClusterLoadBalancer`

Create a rule for port 5672 and 15672:

`az network lb rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name ruleForRabbit --protocol tcp --frontend-port 5672 --backend-port 5672 --frontend-ip-name publicIPforRabbit --backend-pool-name myBackendPool --probe-name rabbitMQProbe`

`az network lb rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name ruleForRabbitMGMT --protocol tcp --frontend-port 15672 --backend-port 15672 --frontend-ip-name publicIPforRabbit --backend-pool-name myBackendPool --probe-name rabbitMQProbe`

Create a way to get to ssh: 

`az network lb inbound-nat-rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name sshRuleOne --protocol tcp --frontend-port 4221 --backend-port 22 --frontend-ip-name publicIpforRabbit`

`az network lb inbound-nat-rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name sshRuleTwo --protocol tcp --frontend-port 4222 --backend-port 22 --frontend-ip-name publicIpforRabbit`

`az network lb inbound-nat-rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name sshRuleThree --protocol tcp --frontend-port 4223 --backend-port 22 --frontend-ip-name publicIpforRabbit`

Create Network Security Group and Rule port 22: 

`az network nsg create -g sandboxGroup --name myNSG`

`az network nsg rule create -g sandboxGroup --nsg-name myNSG --name networkSecurityGroupSSHRule --protocol tcp --direction inbound --source-address-prefix '*' --source-port-range '*' --destination-address-prefix '*' --destination-port-range 22 --access allow --priority 1010`

Create a nsg rule for port 5672 and 15672 

`az network nsg rule create -g sandboxGroup --nsg-name myNSG --name networkSecurityGroupSSHRule --protocol tcp --direction inbound --source-address-prefix '*' --source-port-range '*' --destination-address-prefix '*' --destination-port-range 5672 --access allow --priority 1020`

`az network nsg rule create -g sandboxGroup --nsg-name myNSG --name networkSecurityGroupSSHRule --protocol tcp --direction inbound --source-address-prefix '*' --source-port-range '*' --destination-address-prefix '*' --destination-port-range 15672 --access allow --priority 1030`

Create three Network Cards and associate with load balancer and NSG: 

`az network nic create -g sandboxGroup --name myNic1 --vnet-name sandboxVnet --subnet firstSubnet --network-security-group myNSG --lb-name rabbitClusterLoadBalancer --lb-address-pools myBackendPool --lb-inbound-nat-rules sshRuleOne` 

`az network nic create -g sandboxGroup --name myNic2 --vnet-name sandboxVnet --subnet firstSubnet --network-security-group myNSG --lb-name rabbitClusterLoadBalancer --lb-address-pools myBackendPool --lb-inbound-nat-rules sshRuleTwo`

`az network nic create -g sandboxGroup --name myNic3 --vnet-name sandboxVnet --subnet firstSubnet --network-security-group myNSG --lb-name rabbitClusterLoadBalancer --lb-address-pools myBackendPool --lb-inbound-nat-rules sshRuleThree`

Create availability set: 

`az vm availability-set create -n RabbitMQAvailabilitySet -g sandboxGroup`

Create three vms in our availability set: 

`az vm create -g sandboxGroup -n rabbitNodeOne --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --nics myNic1`

`az vm create -g sandboxGroup -n rabbitNodeTwo --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --nics myNic2`

`az vm create -g sandboxGroup -n rabbitNodeThree --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --nics myNic3`

Let's install rabbitMQ 


### OLD 


Create an availability set: 

`az vm availability-set create -n RabbitMQAvailabilitySet -g sandboxGroup`

Create an Ubuntu VM in said Availability Set: 

`az vm create -g sandboxGroup -n rabbitNodeOne --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --vnet-name GroupVnet --subnet rabbitMQSubnet`

`az vm create -g sandboxGroup -n rabbitNodeTwo --authentication-type password --image UbuntuLTS --availability-set RabbitMQAvailabilitySet --admin-username conductor --admin-password Password1234! --vnet-name GroupVnet --subnet rabbitMQSubnet`

Open our RabbitMQ Ports: 

`az vm open-port -g sandboxGroup -n rabbitNodeOne --port 15672 --priority 350`

`az vm open-port -g sandboxGroup -n rabbitNodeOne --port 5672 --priority 300`

`az vm open-port -g sandboxGroup -n rabbitNodeTwo --port 15672 --priority 350`

`az vm open-port -g sandboxGroup -n rabbitNodeTwo --port 5672 --priority 300`

`az vm open-port -g sandboxGroup -n rabbitNodeOne --port 4369 --priority 400`

`az vm open-port -g sandboxGroup -n rabbitNodeTwo --port 4369 --priority 400`

Get the public IPs of our VMs; 

`az vm list-ip-addresses -g sandboxGroup -o table` 

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

### Configure the cluster: 

In rabbitNodeTwo: 

`sudo rabbitmqctl stop_app`

`sudo rabbitmqctl join_cluster rabbit@rabbitNodeOne`

`sudo rabbitmqctl start_app`

`sudo rabbitmqctl set_cluster_name RabbitCluster`

### [Create an admin user](https://stackoverflow.com/questions/40436425/how-do-i-create-or-add-a-user-to-rabbitmq): 

`sudo rabbitmqctl add_user test test`

`sudo rabbitmqctl set_user_tags  test administrator`

`sudo rabbitmqctl set_permissions -p / test '.*' '.*' '.*'`

### Login to the management portal: 

In the browser go to: 
`http://13.64.99.73:15672/` || `http://your.vm.ip.address:15672` 

- Username: test
- Password: test 

You should see two nodes! 


# TODO: Load balancing configuration! 

### Create a public ip for our cluster: 

`az network public-ip create -g sandboxGroup --name publicIpforRabbitCluster`

Create a load balancer 

`az network lb create -g sandboxGroup -n rabbitClusterLoadBalancer --vnet-name GroupVnet --subnet rabbitMQSubnet --sku Basic --backend-pool-name myBackendPool --frontend-ip-name publicIPforRabbit`

Create a health probe: 

`az network lb probe create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name rabbitHealthProbe --protocol tcp --port 5672`

Create a backend pool: 

`az network lb address-pool create -g sandboxGroup --name rabbitNodeBackend --lb-name rabbitclusterloadbalancer`

Create a rule for your Load Balancer: 

`az network lb rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --protocol tcp --frontend-port 5672 --backend-port 5672 --name rabbitMqLBRule --backend-pool-name rabbitNodeBackend`

Add the VM NIC to the Pool: 

`az network nic ip-config create --name privatePoolConfigOne --nic-name rabbitNodeOnevmnic --lb-address-pools rabbitNodeBackend --lb-name rabbitClusterLoadBalancer -g sandboxGroup`

`az network nic ip-config create --name privatePoolConfigTwo --nic-name rabbitNodeTwoVMNic --lb-address-pools rabbitNodeBackend --lb-name rabbitClusterLoadBalancer -g sandboxGroup`

# Create a public IP for our external load balancer 

`az network public-ip create -n publicIpForRabbitCluster -g sandboxGroup`

# Create a public load balancer: 

`az network lb create --name publicLoadBalancerForRabbit -g sandboxGroup --sku Basic --backend-pool-name rabbitPool --public-ip-address publicIpforrabbitcluster`

# Add our Nics to the LB Address Pool

`az network nic ip-config create --name ipPoolConfigOne --nic-name rabbitNodeOnevmnic --lb-address-pools rabbitPool --lb-name publicLoadBalancerForRabbit -g sandboxGroup`

`az network nic ip-config create --name ipPoolConfigTwo --nic-name rabbitNodeOnevmnic --lb-address-pools rabbitPool --lb-name publicLoadBalancerForRabbit -g sandboxGroup`

# Create our health probe to keep our lb aware: 

`az network lb probe create -g sandboxGroup --lb-name publicLoadBalancerForrabbit --name rabbitHealth --protocol tcp --port 5672`

# Create a rule: 

`az network lb rule create -g sandboxGroup --lb-name publicLoadBalancerForRabbit --protocol tcp --frontend-port 5672 --backend-port 5672 --name rabbitMQRule --backend-pool-name rabbitpool --probe-name rabbitHealth`

# Send some traffic to our load balancer? 




# Create our rule from the public ip to the backend pool

`az network lb rule create -g sandboxGroup --lb-name publicLoadBalancerForRabbit --protocol tcp --frontend-port 5672 --backend-port 5672 --name rabbitMQRule --backend-pool-name rabbitpool`






# JUNK 


`/subscriptions/5c514147-21c3-4f7e-8329-625443da4254/resourceGroups/sandboxGroup/providers/Microsoft.Network/loadBalancers/rabbitClusterLoadBalancer/backendAddressPools/rabbitNodeBackend`

`/subscriptions/5c514147-21c3-4f7e-8329-625443da4254/resourceGroups/sandboxGroup/providers/Microsoft.Network/loadBalancers/rabbitClusterLoadBalancer/loadBalancingRules/rabbitMqLBRule`

`az network nic update -g sandboxGroup -n rabbitnodeonevmnic --add ipConfigurations.loadBalancerBackendAddressPools="/subscriptions/5c514147-21c3-4f7e-8329-625443da4254/resourceGroups/sandboxGroup/providers/Microsoft.Network/loadBalancers/rabbitClusterLoadBalancer/backendAddressPools/rabbitNodeBackend"`

`az network nic ip-config update -g sandboxGroup -n rabbitNodeOnevmnic --lb-name rabbitClusterLoadBalancer --lb-address-pools rabbitNodeBackend`

Create a backend pool to our availability set: 

`az network lb rule create -g sandboxGroup --lb-name rabbitClusterLoadBalancer --name myRabbitRule --protocol tcp --frontend-port 5672 --backend-port 5672 --frontend-ip-name publicIpforRabbitCluster`


References: 
[Docs.Microsoft.com](https://docs.microsoft.com/en-us/)
[RabbitMQ High-availability clusters on Azure VM by Moim Hossain](https://moimhossain.com/2015/01/23/rabbitmq-high-availability-clusters-on-azure-vm/)