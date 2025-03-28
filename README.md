Socket Server in C#

This is a simple multi-client TCP socket server implemented in C#. It accepts numeric input and simple commands from multiple clients simultaneously.

Features

Accepts TCP connections on a specified port.

Handles multiple simultaneous clients.

For each client:

Displays a welcome message.

Accepts line-based input via telnet.

Sums all valid integers entered during the session.

Supports the list command to show all connected clients and their current sum.

Responds to invalid input with an error message.

Cleans up resources on client disconnect.

Requirements

.NET 6 SDK or later

How to Run

Build the project:

dotnet build

Run the server with a port number:

dotnet run --project <YourProjectName> -- 5000

Replace 5000 with the desired port number.

Connect using telnet:

telnet localhost 5000

Usage Examples

Enter 10, 20 → server replies with running sum: Sum: 10, Sum: 30

Enter list → server replies with:

127.0.0.1:12345 : 30
127.0.0.1:12346 : 15

Enter hello → server replies with:

Invalid input. Please enter a number or 'list'.

Notes

Server uses async methods and ConcurrentDictionary for thread-safe client handling.

Only built-in .NET libraries are used.
