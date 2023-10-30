using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

internal class Class10
{
    private byte[] byte_0;

    private uint uint_0;

    private int int_0;

    public Class10(string string_0)
    {
        this.byte_0 = (
            from int_0 in Enumerable.Range(0, string_0.Length)
            where int_0 % 2 == 0
            select Convert.ToByte(string_0.Substring(int_0, 2), 16)).ToArray<byte>();
        this.uint_0 = 0U;
        this.int_0 = this.byte_0.Length;
    }

	// Token: 0x0600004E RID: 78 RVA: 0x0000ECC4 File Offset: 0x0000CEC4
	public int method_0()
	{
		if ((ulong)(this.uint_0 + 4U) > (ulong)((long)this.int_0))
		{
			throw new ApplicationException("read int32 error");
		}
		int result = (int)(this.byte_0[(int)this.uint_0] & byte.MaxValue) | (int)(this.byte_0[(int)(this.uint_0 + 1U)] & byte.MaxValue) << 8 | (int)(this.byte_0[(int)(this.uint_0 + 2U)] & byte.MaxValue) << 16 | (int)(this.byte_0[(int)(this.uint_0 + 3U)] & byte.MaxValue) << 24;
		this.uint_0 += 4U;
		return result;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x0000AFCD File Offset: 0x000091CD
	public uint method_1()
	{
		return (uint)this.method_0();
	}

	// Token: 0x06000050 RID: 80 RVA: 0x0000ED5C File Offset: 0x0000CF5C
	public int method_2()
	{
		if ((ulong)(this.uint_0 + 8U) > (ulong)((long)this.int_0))
		{
			throw new ApplicationException("read int64 error");
		}
		int result = (int)(this.byte_0[(int)this.uint_0] & byte.MaxValue) | (int)(this.byte_0[(int)(this.uint_0 + 1U)] & byte.MaxValue) << 8 | (int)(this.byte_0[(int)(this.uint_0 + 2U)] & byte.MaxValue) << 16 | (int)(this.byte_0[(int)(this.uint_0 + 3U)] & byte.MaxValue) << 24 | (int)(this.byte_0[(int)(this.uint_0 + 4U)] & byte.MaxValue) | (int)(this.byte_0[(int)(this.uint_0 + 5U)] & byte.MaxValue) << 8 | (int)(this.byte_0[(int)(this.uint_0 + 6U)] & byte.MaxValue) << 16 | (int)(this.byte_0[(int)(this.uint_0 + 7U)] & byte.MaxValue) << 24;
		this.uint_0 += 8U;
		return result;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x0000EE54 File Offset: 0x0000D054
	public bool method_3()
	{
		uint num = this.method_1();
		return num == 2574415285U;
	}

	// Token: 0x06000052 RID: 82 RVA: 0x0000EE74 File Offset: 0x0000D074
	public byte[] method_4(uint uint_1)
	{
		if ((ulong)uint_1 > (ulong)((long)this.int_0 - (long)((ulong)this.uint_0)))
		{
			throw new ApplicationException("read bytes error");
		}
		byte[] array = new byte[uint_1];
		Array.Copy(this.byte_0, (long)((ulong)this.uint_0), array, 0L, (long)((ulong)uint_1));
		this.uint_0 += uint_1;
		return array;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x0000EED4 File Offset: 0x0000D0D4
	public string method_5()
	{
		uint num = 1U;
		if ((ulong)(this.uint_0 + 1U) > (ulong)((long)this.int_0))
		{
			throw new ApplicationException("read string error");
		}
		byte[] array = this.byte_0;
		uint num2 = this.uint_0;
		this.uint_0 = num2 + 1U;
		uint num3 = array[(int)num2];
		if (num3 >= 254U)
		{
			if ((ulong)(this.uint_0 + 3U) > (ulong)((long)this.int_0))
			{
				throw new ApplicationException("read string error");
			}
			num3 = (uint)((int)this.byte_0[(int)this.uint_0] | (int)this.byte_0[(int)(this.uint_0 + 1U)] << 8 | (int)this.byte_0[(int)(this.uint_0 + 2U)] << 16);
			this.uint_0 += 3U;
			num = 4U;
		}
		uint num4 = (num3 + num) % 4U;
		if (num4 != 0U)
		{
			num4 = 4U - num4;
		}
		if ((ulong)(this.uint_0 + num3 + num4) > (ulong)((long)this.int_0))
		{
			throw new ApplicationException("read string error");
		}
		string @string = Encoding.UTF8.GetString(this.method_4(num3));
		this.uint_0 += num4;
		return @string;
	}
}