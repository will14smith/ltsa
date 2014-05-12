using System;
using System.Linq;
using System.Linq.Expressions;
using Antlr4.Runtime.Tree;
using LTSASharp.Fsp.Expressions;
using LTSASharp.Parsing;

namespace LTSASharp.Fsp.Conversion
{
    class FspExpressionConverter : FspBaseConverter<FspExpression>
    {
        private readonly FspConverterEnvironment env;

        public FspExpressionConverter(FspConverterEnvironment env)
        {
            this.env = env;
        }

        public override FspExpression VisitExpression(FSPActualParser.ExpressionContext context)
        {
            return context.orExpr().Accept(this);
        }

        public override FspExpression VisitOrExpr(FSPActualParser.OrExprContext context)
        {
            return context.andExpr().Select(x => x.Accept(this)).Aggregate((a, b) => new FspOrExpr(a, b));
        }

        public override FspExpression VisitAndExpr(FSPActualParser.AndExprContext context)
        {
            return context.bitOrExpr().Select(x => x.Accept(this)).Aggregate((a, b) => new FspAndExpr(a, b));
        }

        public override FspExpression VisitBitOrExpr(FSPActualParser.BitOrExprContext context)
        {
            return context.bitExclOrExpr().Select(x => x.Accept(this)).Aggregate((a, b) => new FspBitOrExpr(a, b));
        }

        public override FspExpression VisitBitExclOrExpr(FSPActualParser.BitExclOrExprContext context)
        {
            return context.bitAndExpr().Select(x => x.Accept(this)).Aggregate((a, b) => new FspBitExclOrExpr(a, b));
        }

        public override FspExpression VisitBitAndExpr(FSPActualParser.BitAndExprContext context)
        {
            return context.equalityExpr().Select(x => x.Accept(this)).Aggregate((a, b) => new FspBitAndExpr(a, b));
        }

        public override FspExpression VisitEqualityExpr(FSPActualParser.EqualityExprContext context)
        {
            FspExpression expr = null;

            for (var i = 0; i < context.ChildCount; i++)
            {
                if (i == 0)
                    expr = context.GetChild(i).Accept(this);
                else
                {
                    var op = (ITerminalNode)context.GetChild(i++);
                    var sub = context.GetChild(i).Accept(this);

                    switch (op.Symbol.Type)
                    {
                        case FSPActualLexer.EqualEqual:
                            expr = new FspEqualExpr(expr, sub);
                            break;
                        case FSPActualLexer.NotEqual:
                            expr = new FspNotEqualExpr(expr, sub);
                            break;
                    }
                }
            }

            return expr;
        }

        public override FspExpression VisitRelationalExpr(FSPActualParser.RelationalExprContext context)
        {
            FspExpression left = null;

            for (var i = 0; i < context.ChildCount; i++)
            {
                if (i == 0)
                    left = context.GetChild(i).Accept(this);
                else
                {
                    var op = (ITerminalNode)context.GetChild(i++);
                    var right = context.GetChild(i).Accept(this);

                    switch (op.Symbol.Type)
                    {
                        case FSPActualLexer.Lt:
                            left = new FspLessExpr(left, right);
                            break;
                        case FSPActualLexer.LtEqual:
                            left = new FspLessEqualExpr(left, right);
                            break;
                        case FSPActualLexer.Gt:
                            left = new FspGreaterExpr(left, right);
                            break;
                        case FSPActualLexer.GtEqual:
                            left = new FspGreaterEqualExpr(left, right);
                            break;
                    }
                }
            }

            return left;
        }

        public override FspExpression VisitShiftExpr(FSPActualParser.ShiftExprContext context)
        {
            FspExpression left = null;

            for (var i = 0; i < context.ChildCount; i++)
            {
                if (i == 0)
                    left = context.GetChild(i).Accept(this);
                else
                {
                    var op = (ITerminalNode)context.GetChild(i++);
                    var right = context.GetChild(i).Accept(this);

                    switch (op.Symbol.Type)
                    {
                        case FSPActualLexer.LtLt:
                            left = new FspLeftShiftExpr(left, right);
                            break;
                        case FSPActualLexer.GtGt:
                            left = new FspRightShiftExpr(left, right);
                            break;
                    }
                }
            }

            return left;
        }

        public override FspExpression VisitAdditiveExpr(FSPActualParser.AdditiveExprContext context)
        {
            FspExpression left = null;

            for (var i = 0; i < context.ChildCount; i++)
            {
                if (i == 0)
                    left = context.GetChild(i).Accept(this);
                else
                {
                    var op = (ITerminalNode)context.GetChild(i++);
                    var right = context.GetChild(i).Accept(this);

                    switch (op.Symbol.Type)
                    {
                        case FSPActualLexer.Plus:
                            left = new FspAddExpr(left, right);
                            break;
                        case FSPActualLexer.Minus:
                            left = new FspMinusExpr(left, right);
                            break;
                    }
                }
            }

            return left;
        }

        public override FspExpression VisitMultiplicativeExpr(FSPActualParser.MultiplicativeExprContext context)
        {
            FspExpression left = null;

            for (var i = 0; i < context.ChildCount; i++)
            {
                if (i == 0)
                    left = context.GetChild(i).Accept(this);
                else
                {
                    var op = (ITerminalNode)context.GetChild(i++);
                    var right = context.GetChild(i).Accept(this);

                    switch (op.Symbol.Type)
                    {
                        case FSPActualLexer.Star:
                            left = new FspMultiplyExpr(left, right);
                            break;
                        case FSPActualLexer.Divide:
                            left = new FspDivideExpr(left, right);
                            break;
                        case FSPActualLexer.Modulo:
                            left = new FspModuloExpr(left, right);
                            break;
                    }
                }
            }

            return left;
        }

        public override FspExpression VisitUnaryExpr(FSPActualParser.UnaryExprContext context)
        {
            var expr = context.baseExpr().Accept(this);

            if (context.Minus() != null)
            {
                return new FspNegateExpr(expr);
            }

            if (context.Not() != null)
            {
                return new FspNotExpr(expr);
            }

            //TODO is +expr === expr?
            
            return expr;
        }

        public override FspExpression VisitBaseExpr(FSPActualParser.BaseExprContext context)
        {
            if (context.IntegerLiteral() != null)
            {
                return new FspIntegerExpr(int.Parse(context.IntegerLiteral().GetText()));
            }

            if (context.LowerCaseIdentifier() != null)
            {
                return new FspVariableExpr(context.LowerCaseIdentifier().GetText());
            }

            Unimpl(context.UpperCaseIdentifier());
            Unimpl(context.Quote());
            Unimpl(context.Hash());
            Unimpl(context.At());

            if (context.expression() != null)
            {
                return context.expression().Accept(this);
            }

            throw new InvalidOperationException();
        }
    }
}