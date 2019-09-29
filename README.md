# Highway

## Background
In October 2015 at [JinRi.cn](https://www.jinri.cn/) firm. As the business grows, We have met a big problem order-state changing notifications. However, more than 20 applications get the order-state changed info by scanning Order Database. Someday, requests from scanning and clients almost reached the maximum 32767 concurrent connections. So the database dealing with transactions become very slow because the internal queue is full and thread context switching takes up a lot of CPU time. You know all related things went into a vicious circle. So we stopped all scanning windows services, and appropriately rise maximum threshold of database connections. Two hours later everything looked as good. We just reopen 3  scanning services. So we have made a decision to build a new project “Highway” meaning the Enterprise Message Bus. 
I am responsible for leading the project in design and developing.
I spend five days to design, and we discuss it about 6 times. we developed for 7 days and testing and Load testing took 5 days.
Highway collects all message from the business systems and notifies all registered clients. it decouples message producing and message consuming. Meanwhile, Highway reserve high availability, high concurrency, scalability. And it was launched in December 2015. the Highway is comprised of four SOA services, receiver service, builder service, redo service and notify service. 
Nowadays, It collects nearly 1 million messages and sends out more than 10 millions messages every day. And from receiving to sending, It takes about 1 second.

## Design

### System Structure Diagram
the Highway is comprised with four SOA services, receiver service, builder service, redo service and notify service.
![System Structure Diagram](https://github.com/borllor/Highway/blob/master/Design/System-Architecture.jpg?raw=true "System Structure Diagram")

### Activity Diagram
We use MQ, Redis and concurrent computing to rise performance.
![Activity Diagram](https://github.com/borllor/Highway/blob/master/Design/Activity.jpg?raw=true "Activity Diagram")

## System Screen Shots

### System Configuration
![System Configuration](https://github.com/borllor/Highway/blob/master/Resources/Shots/NotifySetting-1.jpg?raw=true "System Configuration")
![System Configuration](https://github.com/borllor/Highway/blob/master/Resources/Shots/NotifySetting-2.jpg?raw=true "System Configuration")
![System Configuration](https://github.com/borllor/Highway/blob/master/Resources/Shots/NotifySetting-3.jpg?raw=true "System Configuration")

### Client-Interface Configuration
![Client-Interface Configuration](https://github.com/borllor/Highway/blob/master/Resources/Shots/Interfaces-Configuration.jpg?raw=true "Client-Interface Configuration")

### Message-Type Configuration
![Message-Type Configuration](https://github.com/borllor/Highway/blob/master/Resources/Shots/MessageType-Configuration.jpg?raw=true "Message-Type Configuration")

### Message-Received Management
![Message-Received Management](https://github.com/borllor/Highway/blob/master/Resources/Shots/Received-Messages.jpg?raw=true "Message-Received Management")

### Message-Noticed Management
![Message-Noticed Management](https://github.com/borllor/Highway/blob/master/Resources/Shots/Noticed-Messages.jpg?raw=true "Message-Noticed Management")

## Discuss
If you have any question, please feel free to let me know.
### Contact Info
Email: borllor@163.com
City: Auckland, New Zealand.
