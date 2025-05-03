In order to publish to Docker Hub, run the following command from the directory with the Dockerfile:

docker build -t username/dockerhub-project-name:tag-name -f Dockerfile .

Then run:

docker push username/dockerhub-project-name

In order to run on Raspberry Pi:

docker pull username/dockerhub-project-name:tag-name

docker run -d -p 80:80 --name mywebapp username/dockerhub-project-name:tag-name

E.g. docker run -d -p 8008:8080 --dns 8.8.8.8 --dns 8.8.4.4 --hostname home-utilities --name home_utilities_docker -v dataprotection_keys:/home/app/.aspnet/DataProtection-Keys cdosch/homeutilities:latest

     docker run -d -p 8008:8080 --dns 8.8.8.8 --dns 8.8.4.4 --hostname home-utilities --name home_utilities_docker -v dataprotection_keys:/app/.aspnet/DataProtection-Keys cdosch/homeutilities:latest