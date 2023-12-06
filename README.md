# P2P Chat Application

This repo contains a P2P (peer-to-peer) chat app that my partner and I built using C#. The project was part of the course TDDD49 at Linköping University. The goal of this course was to learn more about C#, Visual Studio, and the MVVM design pattern.

Our chat app allows people to talk to each other directly over the internet without needing a central server. It's a true peer-to-peer setup, meaning that chats are private and happen directly between users' own internet connections.

### Key Features
* **Choose Your Network Endpoint**: Users can start the application by selecting an IP address and port to listen for incoming chat requests.

* **Send and Receive Chat Requests**: Once set up, users can send chat requests to others knowing their IP and port, and also receive incoming requests.

* **Simultaneous Conversations**: The application supports having multiple chat conversations simultaneously.

* **Local Conversation Storage**: All chats are stored locally in JSON format.

* **Reconnect Feature**: When a conversation is no longer active, a reconnect button will be visible.

### Installation
1. **Clone the Repository**: First, clone this repository to your local machine using the following command in your preferred terminal:
   ```
   git clone git@github.com:alexandengstrom/p2p-chat-application.git
   ````
2. **Open the Project in Visual Studio**: Open Visual Studio, go to **File > Open > Project/Solution**, and navigate to the folder where you cloned the repo. Select the solution file (ChatApp.sln) to open the project.
3. **Build the Project**: Go to **Build > Build Solution** (or press **Ctrl+Shift+B**). This will compile the project and make sure there are no errors.
4. **Run the Application**: After the build is successful, run the application by pressing **F5** or clicking on the **Start** button in Visual Studio. This will launch the P2P chat application.

### Images
##### Conversation between Grace and Alice.
![Example1](https://github.com/alexandengstrom/p2p-chat-application/assets/123507241/34f782f9-594a-4adb-853c-52e117ec6627)

##### Trying to reconnect to a user that isnt online.
![Example2](https://github.com/alexandengstrom/p2p-chat-application/assets/123507241/2eb79b16-9530-4bea-a5f2-51ef658acb0e)

##### New chat request from Cassandra.
![Example2](https://github.com/alexandengstrom/p2p-chat-application/assets/123507241/78f1eeae-fdc2-4248-bbc9-5be7e5144fd6)

