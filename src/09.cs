using System.Text;

namespace aoc2024;

internal class Day09 : Day
{
    private record struct FileData(int id, int blocks, int pos);
    private readonly List<FileData> files = [];
    private string layout = string.Empty;
    private readonly List<FileData> gaps = [];

    internal override void Parse()
    {
        StringBuilder layoutBuilder = new();
        var text = Util.Parsing.ReadAllText($"{GetDay()}");
        for (int i = 0; i < text.Length; i += 2)
        {
            var fd = new FileData(i / 2, int.Parse(text[i].ToString()), layoutBuilder.Length);
            files.Add(fd);
            for (int j = 0; j < fd.blocks; j++)
            {
                // using % 10 so the length doesn't get screwed up. placeholder for verifying gap lengths.
                layoutBuilder.Append(fd.id % 10);
            }

            if (i + 1 == text.Length)
            {
                break;
            }

            var gapLen = int.Parse(text[i + 1].ToString());
            gaps.Add(new FileData(layoutBuilder.Length, gapLen, layoutBuilder.Length));

            for (int j = 0; j < gapLen; j++)
            {
                layoutBuilder.Append('.');
            }
        }

        layout = layoutBuilder.ToString();
    }

    internal override string Part1()
    {
        long checksum = 0;

        int currFile = 0;
        int currBlock = 0;
        int repackFileIdx = files.Count - 1;
        int repackFileBlock = 0;
        int gapIdx = 0;
        int gapPos = 0;
        var packedLen = files.Sum(f => f.blocks);
        for (int i = 0; i < packedLen; i++)
        {
            if (i >= gaps[gapIdx].id && i < (gaps[gapIdx].id + gaps[gapIdx].blocks) && gapIdx < gaps.Count && gaps[gapIdx].blocks > gapPos)
            {
                checksum += i * files[repackFileIdx].id;
                repackFileBlock++;
                if (repackFileBlock == files[repackFileIdx].blocks)
                {
                    repackFileBlock = 0;
                    repackFileIdx--;
                }

                gapPos++;
                if (gapPos == gaps[gapIdx].blocks)
                {
                    gapPos = 0;
                    gapIdx++;
                    while (gaps[gapIdx].blocks == 0)
                    {
                        gapIdx++;
                    }
                }
            }
            else
            {
                checksum += i * files[currFile].id;
                currBlock++;
                if (currBlock == files[currFile].blocks)
                {
                    currBlock = 0;
                    currFile++;
                }
            }
        }

        return $"Compacted blocks checksum: <+white>{checksum}";
    }

    internal override string Part2()
    {
        List<FileData> packed = [..files];
        List<FileData> newGaps = [..gaps];

        int repackFileIdx = files.Count - 1;
        for (int i = repackFileIdx; i >= 0; i--)
        {
            var firstRepackIdx = newGaps.FindIndex(g => g.pos < files[i].pos && g.blocks >= files[i].blocks);
            if (firstRepackIdx >= 0)
            {
                packed[i] = packed[i] with {pos = newGaps[firstRepackIdx].pos};
                newGaps[firstRepackIdx] = new FileData(newGaps[firstRepackIdx].id + files[i].blocks,
                    newGaps[firstRepackIdx].blocks - files[i].blocks, newGaps[firstRepackIdx].pos + files[i].blocks);
            }
        }

        packed.Sort((a, b) => a.pos.CompareTo(b.pos));

        long checksum = 0;
        foreach (var file in packed)
        {
            for (int i = 0; i < file.blocks; i++)
            {
                checksum += (file.pos + i) * file.id;
            }
        }

        return $"Whole-file defrag checksum: <+white>{checksum}";
    }
}
