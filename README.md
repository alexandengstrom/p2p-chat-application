# p2p-chat-application

This repository hosts a peer-to-peer (P2P) chat application, developed collaboratively in C#. The project serves as an initial exploration into the use of C# and the Model-View-ViewModel (MVVM) design pattern. It is designed to provide practical experience in these areas, demonstrating the application of these technologies in a real-world scenario.

### What is this application about?
Our P2P chat application is a serverless communication tool that enables users to connect directly with each other over the internet. By design, it operates without a central server, embodying the true essence of peer-to-peer networking. This approach ensures that conversations are direct and private, relying solely on the participants' network endpoints.

### Key Features
* **Choose Your Network Endpoint**: Users can start the application by selecting an IP address and port to listen for incoming chat requests. This flexibility allows for adaptable and user-defined connectivity.

* **Send and Receive Chat Requests**: Once set up, users can send chat requests to others knowing their IP and port, and also receive incoming requests. This system upholds the decentralized nature of P2P applications.

* **Simultaneous Conversations**: The application supports having multiple chat conversations simultaneously, catering to dynamic communication needs.

* **Local Conversation Storage**: All chats are stored locally in JSON format. This not only ensures data privacy but also allows for easy access and management of chat histories.

* **Reconnect Feature**: Reflecting on past conversations and wishing to reconnect? Our app allows users to send a chat request to the IP and port used in previous conversations, facilitating reconnection with past chat partners.
