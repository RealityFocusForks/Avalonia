﻿using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.UnitTests;
using Xunit;

namespace Avalonia.Visuals.UnitTests.Media
{
    public class GlyphRunTests : TestWithServicesBase
    {
        public GlyphRunTests()
        {
            AvaloniaLocator.CurrentMutable
                .Bind<IPlatformRenderInterface>().ToSingleton<MockPlatformRenderInterface>();
        }

        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 3, 30)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 1, 0, 10)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 2, 0, 20)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 2, 1, 30)]
        [Theory]
        public void Should_Get_Distance_From_CharacterHit(double[] advances, ushort[] clusters, int start, int trailingLength, double expectedDistance)
        {
            using (var glyphRun = CreateGlyphRun(advances, clusters))
            {
                var characterHit = new CharacterHit(start, trailingLength);

                var distance = glyphRun.GetDistanceFromCharacterHit(characterHit);

                Assert.Equal(expectedDistance, distance);
            }
        }

        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 25.0, 0, 3, true)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 20.0, 2, 0, true)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 26.0, 2, 1, true)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 35.0, 2, 1, false)]
        [Theory]
        public void Should_Get_CharacterHit_FromDistance(double[] advances, ushort[] clusters, double distance, int start,
            int trailingLengthExpected, bool isInsideExpected)
        {
            using (var glyphRun = CreateGlyphRun(advances, clusters))
            {
                var textBounds = glyphRun.GetCharacterHitFromDistance(distance, out var isInside);

                Assert.Equal(start, textBounds.FirstCharacterIndex);

                Assert.Equal(trailingLengthExpected, textBounds.TrailingLength);

                Assert.Equal(isInsideExpected, isInside);
            }
        }

        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0, 3, 30.0)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 1, 2 }, 0, 1, 1, 1, 10.0)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 0, 1, 1, 3 }, 0, 2, 1, 2, 20.0)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 0, 1, 1, 3 }, 0, 1, 1, 2, 20.0)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 3, 1, 1, 0 }, 1, 1, 1, 2, 20.0)]
        [Theory]
        public void Should_Find_Nearest_CharacterHit(double[] advances, ushort[] clusters, int bidiLevel,
            int index, int expectedIndex, int expectedLength, double expectedWidth)
        {
            using (var glyphRun = CreateGlyphRun(advances, clusters, bidiLevel))
            {
                var textBounds = glyphRun.FindNearestCharacterHit(index, out var width);

                Assert.Equal(expectedIndex, textBounds.FirstCharacterIndex);

                Assert.Equal(expectedLength, textBounds.TrailingLength);

                Assert.Equal(expectedWidth, width, 2);
            }
        }

        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0, 3, 0)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0, 3, 1)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 0, 0, 0, 3 }, 3, 0, 3, 1, 0)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 3, 0, 0, 0 }, 3, 0, 3, 1, 1)]
        [InlineData(new double[] { 10, 10, 10, 10, 10 }, new ushort[] { 0, 1, 1, 1, 4 }, 4, 0, 4, 1, 0)]
        [InlineData(new double[] { 10, 10, 10, 10, 10 }, new ushort[] { 4, 1, 1, 1, 0 }, 4, 0, 4, 1, 1)]
        [Theory]
        public void Should_Get_Next_CharacterHit(double[] advances, ushort[] clusters,
            int currentIndex, int currentLength,
            int nextIndex, int nextLength,
            int bidiLevel)
        {
            using (var glyphRun = CreateGlyphRun(advances, clusters, bidiLevel))
            {
                var characterHit = glyphRun.GetNextCaretCharacterHit(new CharacterHit(currentIndex, currentLength));

                Assert.Equal(nextIndex, characterHit.FirstCharacterIndex);

                Assert.Equal(nextLength, characterHit.TrailingLength);
            }
        }

        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0, 0, 0)]
        [InlineData(new double[] { 10, 10, 10 }, new ushort[] { 0, 0, 0 }, 0, 0, 0, 0, 1)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 0, 0, 0, 3 }, 3, 1, 3, 0, 0)]
        [InlineData(new double[] { 10, 10, 10, 10 }, new ushort[] { 3, 0, 0, 0 }, 3, 1, 3, 0, 1)]
        [InlineData(new double[] { 10, 10, 10, 10, 10 }, new ushort[] { 0, 1, 1, 1, 4 }, 4, 1, 4, 0, 0)]
        [InlineData(new double[] { 10, 10, 10, 10, 10 }, new ushort[] { 4, 1, 1, 1, 0 }, 4, 1, 4, 0, 1)]
        [Theory]
        public void Should_Get_Previous_CharacterHit(double[] advances, ushort[] clusters,
            int currentIndex, int currentLength,
            int previousIndex, int previousLength,
            int bidiLevel)
        {
            using (var glyphRun = CreateGlyphRun(advances, clusters, bidiLevel))
            {
                var characterHit = glyphRun.GetPreviousCaretCharacterHit(new CharacterHit(currentIndex, currentLength));

                Assert.Equal(previousIndex, characterHit.FirstCharacterIndex);

                Assert.Equal(previousLength, characterHit.TrailingLength);
            }
        }

        private static GlyphRun CreateGlyphRun(double[] glyphAdvances, ushort[] glyphClusters, int bidiLevel = 0)
        {
            var count = glyphAdvances.Length;
            var glyphIndices = new ushort[count];

            var bounds = new Rect(0, 0, count * 10, 10);

            return new GlyphRun(new GlyphTypeface(new MockGlyphTypeface()), 10, glyphIndices, glyphAdvances,
                glyphClusters: glyphClusters, bidiLevel: bidiLevel, bounds: bounds);
        }
    }
}
