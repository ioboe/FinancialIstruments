# Financial Instrument

This project provides a REST API and WebSocket service for live financial instrument prices.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Postman](https://www.postman.com/downloads/) (for testing REST API)
- `websocat` or `wscat` (for testing WebSocket)

## Getting Started

Follow these steps to set up and run the project in Visual Studio Code on macOS.

### 1. Clone the Repository

Open your terminal and run the following command to clone the repository:

```sh
git clone <your-repo-url>
cd <your-repo-directory>
```

### 2. Install Dependencies
Navigate to the project directory and restore the dependencies:
```sh
cd <your-repo-directory>
dotnet restore
```

### 3. Build the Project
Build the project using the following command:
Navigate to the project directory and restore the dependencies:
```sh
dotnet build
```

### 4. Run the Project
Run the project using the following command:
```sh
dotnet run
```
### 5. Testing the REST API
Use Postman to test the REST API endpoints:

- Get list of instruments:
  - URL: http://localhost:5000/api/financialinstruments/instruments
  - Method: GET
- Get the price of a specific instrument:
  - URL: http://localhost:5000/api/financialinstruments/price/btcusdt
  - Method: GET
  - 
### 6. Testing the WebSocket
Use the browser console or websocat to test WebSocket connections:

- Browser Console:
  - Open the console in Chrome or Firefox.
  - Connect to the WebSocket with the following code:
  ```javascript
    const socket = new WebSocket('ws://localhost:5000/ws/btcusdt');
    socket.onmessage = function(event) {
    console.log('Message from server ', event.data);
    };
    socket.onopen = function(event) {
    console.log('Connected to WebSocket server');
    };
    socket.onclose = function(event) {
    console.log('Disconnected from WebSocket server');
    };
    socket.onerror = function(event) {
    console.error('WebSocket error observed:', event);
  };
  ```
## Running Tests
To run the unit tests, use the following command:
```sh
dotnet test
```
This will execute the tests and show the results.
### Unit Tests
#### REST API Tests
- GetInstruments_ReturnsListOfInstruments: This test checks if the GetInstruments endpoint returns the list of financial instruments.
- GetPrice_ValidInstrument_ReturnsPrice: This test checks if the GetPrice endpoint returns the current price of a valid instrument.
- GetPrice_InvalidInstrument_ReturnsNotFound: This test checks if the GetPrice endpoint returns a 404 status for an invalid instrument.

### WebSocket Tests
- WebSocket_CanConnectAndReceiveMessages: This test checks if a WebSocket client can connect to the WebSocket endpoint and receive messages.
- WebSocket_MultipleClientsCanSubscribe: This test checks if multiple WebSocket clients can subscribe to the same endpoint and receive messages independently.

## Additional Information
For more information on how to use .NET and VSCode, refer to the following resources:
- [Getting Started with .NET](https://docs.microsoft.com/en-us/dotnet/core/get-started)
- [Visual Studio Code Documentation](https://code.visualstudio.com/docs)
