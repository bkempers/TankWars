using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// AUTHOR: Ben Kempers & Anish Narayanaswamy
/// VERSION: March 26, 2021
/// This is a networking library to implement the basics to allow for a server and multiple clients to connect
/// and send and recieve data to the server and back to the clients.
/// </summary>
namespace NetworkUtil
{

    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// A HashSet of Sockets that hold the different clients that have connected to a particular server.
        /// </summary>
        private static HashSet<Socket> clients;

        /// <summary>
        /// A TcpListener that is created when a server is started to allow listeners for the server to implement.
        /// </summary>
        private static TcpListener listener;

        /// <summary>
        /// An Action for the SocketState to allow the user to do something to the data that is sent and recieved from the server.
        /// </summary>
        private static Action<SocketState> OnNetworkAction;
   

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The the port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            clients = new HashSet<Socket>();
            listener = new TcpListener(IPAddress.Any, port);
            OnNetworkAction = toCall;

            listener.Start();

            var ArObject = Tuple.Create(toCall, listener);
            listener.BeginAcceptSocket(AcceptNewClient, ArObject);
            return listener;
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccurred flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            Tuple<Action<SocketState>, TcpListener> ArObject = (Tuple<Action<SocketState>, TcpListener>)ar.AsyncState;
            Action<SocketState> toCall = ArObject.Item1;
            TcpListener serverListener = ArObject.Item2;

            //Tries to finalize the connection and create an event-loop to accept additional clients.
            try
            {
                Socket newClient = serverListener.EndAcceptSocket(ar);
                SocketState state = new SocketState(toCall, newClient);
                toCall(state);

                // Keep track of the client so we can broadcast to all of them
                lock (clients)
                {
                    clients.Add(newClient);
                }

                serverListener.BeginAcceptSocket(AcceptNewClient, ArObject);
            }
            //If there was an error in the connection process and the event-loop has stopped accepting new clients.
            catch (Exception)
            {
                SocketState badState = new SocketState(toCall, null);
                toCall(badState);

                badState.ErrorOccurred = true;
                badState.ErrorMessage = "There was a problem during the connection process, this event loop has stopped.";
            }
        }

        /// <summary>
        /// Stops the given TcpListener.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            //Tries to stop the TcpListener, if there was an error though the TcpListener isn't stopped.
            try
            {
                listener.Stop();
            }
            catch (Exception)
            {
            }
            
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccurred flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Depending on when the error occurs, this should happen either
        /// in this method or in ConnectedCallback.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
        {
            //decoding a host address

            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo;
            IPAddress ipAddress = IPAddress.None;

            // Determine if the server address is a URL or an IP
            try
            {
                ipHostInfo = Dns.GetHostEntry(hostName);
                bool foundIPV4 = false;
                foreach (IPAddress addr in ipHostInfo.AddressList)
                    if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        foundIPV4 = true;
                        ipAddress = addr;
                        break;
                    }
                // Didn't find any IPV4 addresses
                if (!foundIPV4)
                {
                    Socket s = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    SocketState newState = new SocketState(toCall, s)
                    {
                        ErrorOccurred = true,
                        ErrorMessage = "While trying to connect to the server, host name wasnt a IPV4 address."
                    };
                    return;
                }
            }
            catch (Exception)
            {
                // see if host name is a valid ipaddress
                try
                {
                    ipAddress = IPAddress.Parse(hostName);
                }
                catch (Exception)
                {
                    Socket s = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    SocketState newState = new SocketState(toCall, s)
                    {
                        ErrorOccurred = true,
                        ErrorMessage = "While trying to connect to the server, host name isn't a valid IPAddress."
                    };
                    return;
                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            SocketState newSocket = new SocketState(toCall, socket);
            var ArObject = Tuple.Create(toCall, newSocket);

            try
            {
                IAsyncResult result = newSocket.TheSocket.BeginConnect(ipAddress, port, ConnectedCallback, ArObject);

                //Timeout and produce an error when it takes more than 3 seconds to connect
                bool signalRec = result.AsyncWaitHandle.WaitOne(3000, true);

                //signal received
                if (!signalRec)
                {
                    socket.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            Tuple<Action<SocketState>, SocketState> ArObject = (Tuple<Action<SocketState>, SocketState>)ar.AsyncState;
            Action<SocketState> toCall = ArObject.Item1;
            SocketState state = ArObject.Item2;

            //Tries to finalize the connection and call the Action delegate on the SocketState.
            try
            {
                state.TheSocket.EndConnect(ar);

                toCall(state);
            }
            //If there was an error finalizing the connection of the socket.
            catch (Exception)
            {
                state.ErrorOccurred = true;
                state.ErrorMessage = "There was a problem trying to connect to the server.";

                toCall(state);
            }

        }


        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccurred flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Depending on when the error occurs, this should happen either
        /// in this method or in ReceiveCallback.
        /// </summary>
        /// <param name="state">The SocketState to begin receiving</param>
        public static void GetData(SocketState state)
        {
            //Tries to get the data and beging the event-loop of recieving data.
            try
            {
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);
            }
            //If there was an error with the recieve process, the event-loop stops and this SocketState's error is called.
            catch (Exception)
            {
                state.ErrorOccurred = true;
                state.ErrorMessage = "There was an error getting the data associated with the socket " + state.TheSocket.ToString();
                state.OnNetworkAction(state);
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;

            //Tries to finalize the recieve process & collect and gather the data passed to the socket.
            try
            {
                int numBytes = state.TheSocket.EndReceive(ar);
                if(numBytes == 0)
                {
                    state.ErrorOccurred = true;
                    state.ErrorMessage = "There was an error getting the data associated with the socket";
                    state.OnNetworkAction(state);
                    return;
                }

                
                string data = Encoding.UTF8.GetString(state.buffer, 0, numBytes);
                // Buffer the data received (we may not have a full message yet)

                //add a lock to avoid race condition
                lock(state.data)
                {
                    state.data.Append(data);
                    
                }
                //never in lock
                state.OnNetworkAction(state);
                return;

            }
            //If there was an error recieving data to the SocketState, the event loop is stopped and this SocketState's error is called.
            catch (Exception)
            {
                state.ErrorOccurred = true;
                state.ErrorMessage = "There was an error getting the data associated with the socket " + state.TheSocket.ToString();
                state.OnNetworkAction(state);
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {
            //Tries to initiate the send process to the socket.
            try
            {
                if (socket.Connected)
                {
                    SocketState state = new SocketState(OnNetworkAction, socket);

                    byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                    state.TheSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendCallback, socket);
                    return true;
                }
                else
                    return false;
            }
            //If there was an error in the send process, the socket is closed.
            catch (Exception)
            {
                socket.Close();
                return false;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occurred during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            //Tries to finalize the send process to this socket.
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndSend(ar);
            }
            //If an error occurs in the send process, this doesn't throw an error.
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            //Tries to initiate the send process to the socket, then close the socket.
            try
            {
                if (socket.Connected)
                {
                    SocketState state = new SocketState(OnNetworkAction, socket);

                    byte[] messageBytes = Encoding.UTF8.GetBytes(data);
                    state.TheSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, SendAndCloseCallback, socket);

                    //modification
                    socket.Close();
                    return true;
                }
                else
                    return false;
            }
            //If an error occurs during the send process, the socket closes.
            catch (Exception)
            {
                socket.Close();
                return false;
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occurred during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            //Tries to finalize the send and close process.
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndSend(ar);
                socket.Close();
            }
            //If an error occurs during the send and close process, an error doesn't throw.
            catch (Exception)
            {
            }
        }
    }
}
