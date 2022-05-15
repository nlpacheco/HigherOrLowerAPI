# HigherOrLowerAPI


This project is a Higher or Lower Game API for testing purpose.
The project is written using Microsoft Asp Net 6, EF6 Core and C# 9.



## Solution

The Solution has 3 projects:
- HigherOrLowerAPI: it is responsible for asp net startup and rest implementation.
- HigherOrLowerData: it is responsible for gaming logic and database repository entities.
- TestProject.HigherOrLower: it is the test project.


### Game API

The API specification only required human text messages. 

This is not the best kind of back-end API. Usually, the texts and translations are a front-end responsibility.

Therefore, the solution API calls always respond to client using a json with user message as required but also enough properties to correctly communicate to user what is happening.


### Database

The Entity Framework is configured to use MySQL 8 but not migrations. 
Production Database is too important to rely on migrations blindly.

For testing, the project uses SQLite in-memory database to speed up the use case tests.

To create a MySQL 8 instance, there is a docker-compose file available: docker-compose-dev.yml

There are two scripts to do that easily: 

**Windows**
- start-docker-mysql-dev.bat
- stop-docker-mysql-dev.bat

**Linux**
- start-docker-mysql-dev.sh
- stop-docker-mysql-dev.sh

Or to start simply type: 

`docker-compose -f docker-compose-dev.yml up -d`


To stop: 

`docker-compose -f docker-compose-dev.yml down`



### Tests


The tests are too extensive only for completeness. 
Depending on a larger IT strategy, the tests can be more focus on local tests or external interface only.

The game logic is tested using Moq4 and a class called FakeRandomNumber.
That class helps creating a well-known deck of card instead a randomly shuffled deck. 



### Docker and Docker-compose


The solution can be built and can run using a docker container.


*Note: the docker scripts have been tested only on a Linux machine.*


The docker-compose.yml instantiates the MySQL, builds and starts the application image.

The building process is done by the dockerfile.buildGame.yml that uses a Microsoft sdk image to build an application image.

The current building process create a release bin but it is not ready for production yet. 
A lot of improvements are necessary such as global logging configuration, disable swagger and EF logging, open password in config file, certificates not installed, etc.



To run the application using docker: 

```
docker-compose up -d
```



The script `buildGame` is a shortcut to force the application rebuilding.

buildGame:

`docker build -f dockerfile.buildGame.yml -t nlpacheco/higherlowergameapi .`


### Swagger

The Swagger is enable even using the docker build.

It is available at *baseURL*/swagger.

That is the url when the application is running via docker-compose.yml: 

http://localhost:5000/swagger








