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
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
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
                //await SendStringAsync(socket, "ping", ct);
                string[] output = new string[] { "" };
                var srouce = SourceText.From(String.Join(Environment.NewLine, code), "playground.ncodi");
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
                    Action<string> send = async (txt) =>
                    {
                        await SendStringAsync(socket, txt, ct);
                    };
                    var timeLimit=(code.Split("a9ra()").Length - 1)*15000+1000;
                    executionResult = ExecuteCode(compilation,get, send, timeLimit);
                    var result = executionResult.Item2;
                    if (!executionResult.Item1)
                    {
                        await SendStringAsync(socket, $"\n Time limit {timeLimit/1000} sec exceeded", ct);
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", ct);
                        return;
                    }
                    if (!result.Diagnostics.Any())
                    {
                        //if (result.OutputLines.Count() != 0)
                        //{
                        //    output = result.OutputLines.ToArray();
                        //}
                        //else
                        //{
                        //    output = new string[] { "" };
                        //}
                    }
                    else
                    {
                        result.Diagnostics.ReturnDiagnostics(send);
                    }
                }
                catch
                {
                    output = new string[] { "Can't execute this code because it causes an internal error." };
                }
                foreach(var line in output)
                {
                    if(!string.IsNullOrWhiteSpace(line))
                        await SendStringAsync(socket, line, ct);
                }
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
        public (bool, EvaluationResult) ExecuteCode(Compilation compilation, Func<Task<string>> GetInput, Action<string> send,int waitTime)
        {
            EvaluationResult result = null;
            var task = Task.Run(() =>
            {
                result = compilation.Evaluate(new Dictionary<VariableSymbol, object>(), false,GetInput,send);
            });
            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(waitTime));

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