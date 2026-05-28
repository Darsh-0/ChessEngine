using System;

namespace chessEngine.MoveGeneration.MagicBitBoards;

public static class MagicBitboards
{
    
    
    public static ulong[] RookMasks = new ulong[64];
    public static ulong[] RookMagics = new ulong[64];
    public static ulong[][] RookTable = new ulong[64][];
    public static int[]  RookShifts = new int[64];
    
    public static ulong[] BishopMasks = new ulong[64];
    public static ulong[] BishopMagics = new ulong[64];
    public static ulong[][] BishopTable = new ulong[64][];
    public static int[] BishopShifts = new int[64];
    
    static MagicBitboards()
    {
        Initialize();
    }

    static ulong GenerateRookMask(int sq)
    {
        ulong mask = 0;
        int rank = sq / 8, file = sq % 8;
        for (int f = file + 1; f <= 6; f++) mask |= 1UL << (rank * 8 + f);
        for (int f = file - 1; f >= 1; f--) mask |= 1UL << (rank * 8 + f);
        for (int r = rank + 1; r <= 6; r++) mask |= 1UL << (r * 8 + file);
        for (int r = rank - 1; r >= 1; r--) mask |= 1UL << (r * 8 + file);
        return mask;
    }
    
    static ulong ComputeRookAttacks(int sq, ulong occupancy)
    {
        ulong attacks = 0;
        int rank = sq / 8, file = sq % 8;

        for (int r = rank + 1; r < 8; r++)
        {
            attacks |= 1UL << (r * 8 + file);
            if ((occupancy & (1UL << (r * 8 + file))) != 0) break;
        }
        for (int r = rank - 1; r >= 0; r--)
        {
            attacks |= 1UL << (r * 8 + file);
            if ((occupancy & (1UL << (r * 8 + file))) != 0) break;
        }
        for (int f = file + 1; f < 8; f++)
        {
            attacks |= 1UL << (rank * 8 + f);
            if ((occupancy & (1UL << (rank * 8 + f))) != 0) break;
        }
        for (int f = file - 1; f >= 0; f--)
        {
            attacks |= 1UL << (rank * 8 + f);
            if ((occupancy & (1UL << (rank * 8 + f))) != 0) break;
        }
        return attacks;
    }
    
    static ulong GenerateBishopMask(int sq)
    {
        ulong mask = 0;

        int rank = sq / 8;
        int file = sq % 8;
        
        for (int r = rank + 1, f = file + 1; r <= 6 && f <= 6; r++, f++)
            mask |= 1UL << (r * 8 + f);
        for (int r = rank + 1, f = file - 1; r <= 6 && f >= 1; r++, f--)
            mask |= 1UL << (r * 8 + f);
        for (int r = rank - 1, f = file + 1; r >= 1 && f <= 6; r--, f++)
            mask |= 1UL << (r * 8 + f);
        for (int r = rank - 1, f = file - 1; r >= 1 && f >= 1; r--, f--)
            mask |= 1UL << (r * 8 + f);
        return mask;
    }

    static ulong ComputeBishopAttacks(int sq, ulong occupancy)
    {
        ulong attacks = 0;
        int rank = sq / 8;
        int file = sq % 8;

        for (int r = rank + 1, f = file + 1; r < 8 && f < 8; r++, f++)
        {
            int s = r * 8 + f;
            attacks |= 1UL << s;
            if ((occupancy & (1UL << s)) != 0)
            {
                break;
            }
        }
        for (int r = rank + 1, f = file - 1; r < 8 && f >= 0; r++, f--)
        {
            int s = r * 8 + f;
            attacks |= 1UL << s;
            if ((occupancy & (1UL << s)) != 0)
            {
                break;
            }
        }

        for (int r = rank - 1, f = file + 1; r >= 0 && f < 8; r--, f++)
        {
            int s = r * 8 + f;
            attacks |= 1UL << s;
            if ((occupancy & (1UL << s)) != 0)
            {
                break;
            }
        }
        for (int r = rank - 1, f = file - 1; r >= 0 && f >= 0; r--, f--)
        {
            int s = r * 8 + f;
            attacks |= 1UL << s;
            if ((occupancy & (1UL << s)) != 0)
            {
                break;
            }
        }
        return attacks;
    }
    
    static Random rng = new Random(12345);

    static ulong RandomSparse() =>
        (ulong)rng.NextInt64() & (ulong)rng.NextInt64() & (ulong)rng.NextInt64();

    static int PopCount(ulong x)
    {
        int count = 0;
        while (x != 0) { count++; x &= x - 1; }
        return count;
    }

    static void FindRookMagic(int sq)
    {
        ulong mask = RookMasks[sq];
        int bits  = PopCount(mask);
        int shift = 64 - bits;
        int size  = 1 << bits;
        
        ulong[] occupancies = new ulong[size];
        ulong[] attacks     = new ulong[size];
        ulong subset = 0;
        for (int i = 0; i < size; i++)
        {
            occupancies[i] = subset;
            attacks[i]     = ComputeRookAttacks(sq, subset);
            subset = (subset - mask) & mask;
        }

        // try random magic numbers until one produces no collisions
        ulong[] table = new ulong[size];
        while (true)
        {
            ulong magic = RandomSparse();
            Array.Clear(table, 0, size);
            bool failed = false;

            for (int i = 0; i < size; i++)
            {
                int index = (int)((occupancies[i] * magic) >> shift);
                if (table[index] == 0)
                    table[index] = attacks[i];
                else if (table[index] != attacks[i])
                { failed = true; break; }
            }

            if (!failed)
            {
                RookMagics[sq] = magic;
                RookShifts[sq] = shift;
                RookTable[sq]  = table;
                return;
            }
        }
    }
    
    static void FindBishopMagic(int sq)
    {
        ulong mask = BishopMasks[sq];

        int bits  = PopCount(mask);
        int shift = 64 - bits;
        int size  = 1 << bits;

        ulong[] occupancies = new ulong[size];
        ulong[] attacks     = new ulong[size];

        ulong subset = 0;

        for (int i = 0; i < size; i++)
        {
            occupancies[i] = subset;
            attacks[i] = ComputeBishopAttacks(sq, subset);

            subset = (subset - mask) & mask;
        }

        ulong[] table = new ulong[size];

        while (true)
        {
            ulong magic = RandomSparse();

            Array.Clear(table, 0, size);

            bool failed = false;

            for (int i = 0; i < size; i++)
            {
                int index = (int)((occupancies[i] * magic) >> shift);

                if (table[index] == 0)
                {
                    table[index] = attacks[i];
                }
                else if (table[index] != attacks[i])
                {
                    failed = true;
                    break;
                }
            }

            if (!failed)
            {
                BishopMagics[sq] = magic;
                BishopShifts[sq] = shift;
                BishopTable[sq] = table;

                return;
            }
        }
    }
    
    public static ulong GetRookAttacks(int sq, ulong occupancy)
    {
        ulong index = (occupancy & MagicBitboards.RookMasks[sq])
                      * MagicBitboards.RookMagics[sq]
                      >> MagicBitboards.RookShifts[sq];
        return MagicBitboards.RookTable[sq][index];
    }
    
    public static ulong GetBishopAttacks(int sq, ulong occupancy)
    {
        int index = (int)(
            ((occupancy & BishopMasks[sq]) * BishopMagics[sq])
            >> BishopShifts[sq]
        );

        return BishopTable[sq][index];
    }

    public static ulong GetQueenAttacks(int sq, ulong occupancy)
    {
        return GetRookAttacks(sq, occupancy) | GetBishopAttacks(sq, occupancy);
    }
    
    public static int BitIndex(ulong bit) => System.Numerics.BitOperations.TrailingZeroCount(bit);

    public static void Initialize()
    {
        for (int sq = 0; sq < 64; sq++)
        {
            RookMasks[sq] = GenerateRookMask(sq);
            FindRookMagic(sq);
            BishopMasks[sq] = GenerateBishopMask(sq);
            FindBishopMagic(sq);
        }
    }
}