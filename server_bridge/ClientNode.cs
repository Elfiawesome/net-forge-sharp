using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetForge.Network;


public partial class ClientNode : RefCounted
{
    private TcpClient? tcpClient;
    private CancellationTokenSource? _cts;

    public ClientNode()
    {
        // It's good practice to initialize PacketFactory if client might create/understand packets
        // However, for receiving, it's primarily server_core/Network/Packet.cs's PacketFactory.CreatePacket that's used.
        // If the client needs to understand packet types for any reason other than HelloWorldPacket explicitly, ensure it's initialized.
        // PacketFactory.Initialize(); // Usually done on server-side. Client relies on server sending known types.
    }

    public void ConnectToServer()
    {
        _cts = new CancellationTokenSource();
        // Run the entire connect and then receive sequence in one task
        Task.Run(async () =>
        {
            try
            {
                tcpClient = new TcpClient();
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 3115);

                GD.Print("Client: Attempting to connect...");
                // Await the connection attempt and allow cancellation.
                // Use WaitAsync for timeout/cancellation with ConnectAsync(IPEndPoint)
                // Or, if using .NET 5+, ConnectAsync(IPEndPoint, CancellationToken) exists.
                // Assuming your Godot .NET version maps to something that has a CancellationToken overload for ConnectAsync(IPEndPoint, CancellationToken)
                // or you use a helper/WaitAsync. For ConnectAsync(IPAddress, int, CancellationToken) (requires .NET 7+)
                await tcpClient.ConnectAsync(ipEndPoint, _cts.Token);

                GD.Print("Client: Connected to server.");

                // Now that we are connected, start the receive task.
                // This task will run for the lifetime of the connection.
                await ReceiveDataTask(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                GD.Print("Client: Connection or receive operation was cancelled.");
            }
            catch (SocketException ex)
            {
                 GD.Print($"Client: SocketException during connect or receive: {ex.Message} (SocketError: {ex.SocketErrorCode})");
            }
            catch (Exception ex)
            {
                GD.Print($"Client: Error during connect or receive: {ex.Message} Stack: {ex.StackTrace}");
            }
            finally
            {
                GD.Print("Client: Connection and receive task loop ended.");
                CloseConnection();
            }
        }, _cts.Token); // Pass token to Task.Run also, for cancellation of the task itself if not started
    }

    public void LeaveServer()
    {
        GD.Print("Client: LeaveServer called.");
        CloseConnection();
    }

    private void CloseConnection()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }
        tcpClient?.Close(); // Close the TcpClient
        tcpClient = null;
        _cts?.Dispose();
        _cts = null;
        GD.Print("Client: Connection closed and resources released.");
    }

    public async Task ReceiveDataTask(CancellationToken token)
    {
        if (tcpClient == null || !tcpClient.Connected)
        {
            GD.PrintErr("Client: ReceiveDataTask called but TcpClient is null or not connected.");
            return;
        }

        NetworkStream? stream = null;
        try
        {
            stream = tcpClient.GetStream();
            // Create PacketStream once for the duration of this receive loop
            var packetStream = new PacketStream(stream);
            GD.Print($"Client: Starting ReceiveDataTask. Waiting for packets....");

            while (!token.IsCancellationRequested && tcpClient.Connected)
            {
                // GD.Print("Client: Attempting to receive packet..."); // Uncomment for verbose logging
                BasePacket? packet = await packetStream.GetPacketAsync(token);

                if (packet == null)
                {
                    // GetPacketAsync returning null might indicate a graceful shutdown or an error it handled internally.
                    GD.Print("Client: Received null packet. Connection likely closed by server or stream ended.");
                    break; // Exit loop
                }

                // GD.Print($"Client: Received packet from Server! Type: {packet.GetType().Name}, ID: {packet.PacketId}");
                if (packet is HelloWorldPacket helloWorldPacket)
                {
                    GD.Print($"Client: Server says: '{helloWorldPacket.Message}'");
                }
                // TODO: Add handling for other expected packet types here
                // else
                // {
                //     GD.Print($"Client: Received unhandled packet type: {packet.GetType().Name}");
                // }
            }
        }
        catch (OperationCanceledException)
        {
            GD.Print("Client: ReceiveDataTask was cancelled.");
        }
        catch (System.IO.IOException ex) // Often indicates connection closed issues
        {
            GD.Print($"Client: IOException in ReceiveDataTask (connection likely closed): {ex.Message}");
        }
        catch (SocketException ex)
        {
            GD.Print($"Client: SocketException in ReceiveDataTask: {ex.Message} (SocketError: {ex.SocketErrorCode})");
        }
        catch (Exception ex)
        {
            GD.Print($"Client: Error in ReceiveDataTask: {ex.Message} - StackTrace: {ex.StackTrace}");
        }
        finally
        {
            GD.Print("Client: ReceiveDataTask finished.");
            // Connection cleanup is handled by the finally block in ConnectToServer's task or by LeaveServer.
        }
    }

    public void SendData(string data) // 'data' param is not used, HelloWorldPacket has hardcoded message.
    {
        if (tcpClient == null || !tcpClient.Connected || _cts == null || _cts.IsCancellationRequested)
        {
            GD.PrintErr("Client: Cannot send data, client not connected or cancellation requested.");
            return;
        }

        try
        {
            // TODO: Consider creating PacketStream once per connection and reusing it
            // For now, this matches the original structure.
            var packetStream = new PacketStream(tcpClient.GetStream());
            // Fire and forget is okay for sending if you don't need to wait for ack, but log errors.
            _ = packetStream.SendPacketAsync(new HelloWorldPacket("This is a very cool message 👍"), _cts.Token)
                .ContinueWith(t => {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        GD.PrintErr($"Client: Error sending data: {t.Exception.InnerException?.Message ?? t.Exception.Message}");
                    }
                }, TaskScheduler.Default);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Client: Exception while trying to send data: {ex.Message}");
        }
    }
}