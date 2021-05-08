using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ncodi.CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        public ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockEdge> edges)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Edges = edges;
        }

        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockEdge> Edges { get; }

        public sealed class BasicBlock
        {
            public BasicBlock()
            {
            }

            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }
            public bool IsStart { get; }
            public bool IsEnd { get; }
            public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
            public List<BasicBlockEdge> Incoming { get; } = new List<BasicBlockEdge>();
            public List<BasicBlockEdge> Outgoing { get; } = new List<BasicBlockEdge>();

            public override string ToString()
            {
                if (IsStart)
                    return "<Start>";

                if (IsEnd)
                    return "<End>";

                using (var writer = new StringWriter())
                {
                    foreach (var statement in Statements)
                        statement.WriteTo(writer);

                    return writer.ToString();
                }
            }
        }

        public sealed class BasicBlockEdge
        {
            public BasicBlockEdge( BasicBlock from, BasicBlock to, BoundExpression condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression Condition { get; }
        }
        public sealed class BasicBlockBuilder
        {
            private List<BoundStatement> _statements = new List<BoundStatement>();
            private List<BasicBlock> _blocks = new List<BasicBlock>();
            public List<BasicBlock> Build(BoundBlockStatement block)
            {
                foreach (var statement in block.Statements)
                {
                    switch (statement.Kind)
                    {
                        case BoundNodeKind.LabelStatement:
                            StartBlock();
                            _statements.Add(statement);
                            break;
                        case BoundNodeKind.GotoStatement:
                        case BoundNodeKind.ConditionalGotoStatement:
                        case BoundNodeKind.ReturnStatement:
                            _statements.Add(statement);
                            StartBlock();
                            break;
                        case BoundNodeKind.VariableDeclaration:
                        case BoundNodeKind.ExpressionStatement:
                            _statements.Add(statement);
                            break;
                        default:
                            throw new Exception($"Unexpected statement: {statement.Kind}");
                    }
                    EndBlock();
                }
                return _blocks.ToList();
            }
            private void EndBlock()
            {
                if(_statements.Count > 0)
                {
                    var block = new BasicBlock();
                    block.Statements.AddRange(_statements);
                    _blocks.Add(block);
                    _statements.Clear();
                }
            }
            private void StartBlock()
            {
                EndBlock();
            }
        }
        public void WriteTo(TextWriter writer)
        {
            writer.WriteLine("digraph G {");

            var blockIds = new Dictionary<BasicBlock, string>();
            for (int i = 0; i < Blocks.Count; i++)
            {
                var id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }
            foreach (var block in Blocks)
            {
                var id = blockIds[block];
                var label = block.ToString().Replace(Environment.NewLine, "\\l");
                writer.WriteLine($"    {id} [label = \"{label}\" shape = box]");
            }
            foreach (var edge in Edges)
            {
                var fromId = blockIds[edge.From];
                var toId = blockIds[edge.To];
                var label = edge.Condition == null? string.Empty:edge.Condition.ToString();
                writer.WriteLine($"    {fromId} -> {toId} [label = \"{label}\"]");
            }
            writer.WriteLine("}");
        }
        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            var start = new BasicBlock(isStart:true);
            var end = new BasicBlock(isStart:false);
            var basicBlockBuilder = new BasicBlockBuilder();
            var blocks = basicBlockBuilder.Build(body);
            blocks.Insert(0, start);
            blocks.Add(end);

            return new ControlFlowGraph(start,end,blocks,new List<BasicBlockEdge>());
        }
    }
}
