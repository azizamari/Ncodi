using Microsoft.AspNetCore.Http;
using Ncodi.CodeAnalysis;
using Ncodi.CodeAnalysis.Syntax;
using Ncodi.CodeAnalysis.Text;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using Ncodi.CodeAnalysis.IO;
using Ncodi.CodeAnalysis.Symbols;
using System.Collections.Generic;

namespace Ncodi.Web
{
    public class SampleWebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public SampleWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                // Not a web socket request
                await _next.Invoke(context);
                return;
            }

            var ct = context.RequestAborted;
            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                var code = await ReceiveStringAsync(socket, ct);
                code=code.Replace(',', '\n');
                //await SendStringAsync(socket, "ping", ct);
                string[] output = new string[] { "Code hase no output" };
                var srouce = SourceText.From(String.Join(Environment.NewLine, code), "fileName.ncodi");
                var syntaxTree = SyntaxTree.Parse(srouce);
                var compilation = new Compilation(syntaxTree);
                (bool, EvaluationResult) executionResult;
                try
                {
                    Func<Task<string>> get = async () =>
                    {
                        await SendStringAsync(socket,"send data",ct);
                        var x = await ReceiveStringAsync(socket, ct);
                        return x;
                    };
                    executionResult = ExecuteCode(compilation,get);
                    var result = executionResult.Item2;
                    if (!executionResult.Item1)
                    {
                        await SendStringAsync(socket, "Time limit exceeded", ct);
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", ct);
                        return;
                    }
                    if (!result.Diagnostics.Any())
                    {
                        if (result.OutputLines.Count() != 0)
                        {
                            output = result.OutputLines.ToArray();
                        }
                        else
                        {
                            output = new string[] { "Your code doesn't return any data" };
                        }
                    }
                    else
                    {
                        output = result.Diagnostics.ReturnDiagnostics().Split('\n');
                    }
                }
                catch
                {
                    output = new string[] { "Can't execute this code because it causes an internal error, this is probably our mistake." };
                }
                await SendStringAsync(socket, string.Join('\n',output), ct);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", ct);
                return;
            }
        }

        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default)
        {
            // Message can be sent by chunk.
            // We must read all chunks before decoding the content
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                    throw new Exception("Unexpected message");

                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
        public (bool, EvaluationResult) ExecuteCode(Compilation compilation, Func<Task<string>> GetInput)
        {
            EvaluationResult result = null;
            var task = Task.Run(() =>
            {
                result = compilation.Evaluate(new Dictionary<VariableSymbol, object>(), false,GetInput);
            });
            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(100000));

            if (isCompletedSuccessfully)
            {
                return (true, result);
            }
            else
            {
                return (false, null);
            }
        }
    }
}