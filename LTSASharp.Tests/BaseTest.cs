using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using LTSASharp.Fsp;
using LTSASharp.Fsp.Conversion;
using LTSASharp.Lts;
using LTSASharp.Lts.Conversion;
using LTSASharp.Parsing;
using LTSASharp.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LTSASharp.Tests
{
    public abstract class BaseTest
    {
        protected FspDescription CompileFsp(string input)
        {
            return CompileFsp(new AntlrInputStream(input));
        }

        protected FspDescription CompileFsp(AntlrInputStream input)
        {
            var lexer = new FSPActualLexer(input);
            var parser = new FSPActualParser(new BufferedTokenStream(lexer));

            var fspConverter = new FspConveter();
            parser.fsp_description().Accept(fspConverter);

            return fspConverter.Description;
        }

        protected LtsDescription CompileLts(FspDescription fsp)
        {
            var ltsConverter = new LtsConverter(fsp);

            return ltsConverter.Convert();
        }

        protected void AssertSystemsEquals(LtsDescription lts, params string[] systems)
        {
            var present = lts.Systems.Keys.ToList();

            foreach (var system in systems)
            {
                if (!present.Remove(system))
                    Assert.Fail("System '{0}' is not present", system);
            }

            if (present.Any())
                Assert.Inconclusive("Additional systems {{{0}}} are present", string.Join(", ", present));
        }

        protected void AssertAlphabetContains(LtsSystem lts, params string[] alphabet)
        {
            var expectedAlphabet = alphabet.Select(x => new LtsLabel(x)).ToSet();

            var message = string.Format(
                "Expected alphabet {{{0}}} was not a subset of the actual alphabet {{{1}}}, {{{2}}} was missing",
                string.Join(", ", expectedAlphabet),
                string.Join(", ", lts.Alphabet),
                string.Join(", ", expectedAlphabet.Except(lts.Alphabet))
                );

            Assert.IsTrue(expectedAlphabet.IsSubsetOf(lts.Alphabet), message);
        }

        protected void AssertAlphabetEquals(LtsSystem lts, params string[] alphabet)
        {
            var expectedAlphabet = alphabet.Select(x => new LtsLabel(x)).ToSet();

            var extra = lts.Alphabet.Except(expectedAlphabet).ToSet();
            var missing = expectedAlphabet.Except(lts.Alphabet).ToSet();

            var message = string.Format(
                "Expected alphabet {{{0}}} was not a subset of the actual alphabet {{{1}}}",
                string.Join(", ", expectedAlphabet),
                string.Join(", ", lts.Alphabet)
                );

            if (missing.Count > 0)
                message += string.Format(", {{{0}}} was missing", string.Join(", ", missing));
            if (extra.Count > 0)
                message += string.Format(", {{{0}}} was extra", string.Join(", ", extra));

            Assert.IsTrue(extra.Count == 0 && missing.Count == 0, message);
        }

        protected void AssertStateCountEquals(LtsSystem lts, int expectedCount)
        {
            Assert.AreEqual(expectedCount, lts.States.Count);
        }

        protected void AssertLtsEqual(LtsDescription a, LtsDescription b)
        {
            AssertSystemsEquals(b, a.Systems.Keys.ToArray());
            foreach (var s in a.Systems)
            {
                var ab = s.Value;
                var lb = b.Systems[s.Key];

                AssertStateCountEquals(lb, ab.States.Count);
                AssertAlphabetEquals(lb, ab.Alphabet.Select(x => x.Name).ToArray());
                //TODO transitions
            }
        }
    }
}
