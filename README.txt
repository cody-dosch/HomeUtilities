In order to publish to Docker Hub, run the following command from the directory with the Dockerfile:

	docker build -t username/dockerhub-project-name:tag-name -f Dockerfile .

Then run:

	docker push username/dockerhub-project-name

---------------------------------------------------------------------------------------------------

In order to run on Raspberry Pi:

	docker pull username/dockerhub-project-name:tag-name

	docker run -d -p 8080:8080 --name mywebapp username/dockerhub-project-name:tag-name

	E.g. docker run -d -p 8080:8080 --dns 8.8.8.8 --dns 8.8.4.4 -e ASPNETCORE_ENVIRONMENT=Development --hostname home-utilities --name home_utilities_docker cdosch/homeutilities:latest
	
----------------------------------------------------------------------------------------------------

In order to give it a friendly local DNS name:

	1. Set up Pi-Hole on your raspberry pi.
	2. Set up a local DNS entry in Pi-Hole mapping your chosen friendly hostname to the raspberry pi's IP address.
	3. Set your router's DNS server to your raspberry pi's IP address (make sure this is a static IP)
	
	NOTE: On iPhones, you may also need to take some additional steps to get the friendly hostname to resolve:
		- Disable iCloud Private Relay
		- Disable private IP address assignment on your Wifi network
		- Forget your Wifi network connection and reconnect in order to flush DNS cache
		- Most importantly, may need to change your network's DNS setting on your phone to Manual, and specify your
		  raspberry pi's IP address as first in the list. You can keep the others, e.g. 2001:558:feed::1