using easy;
using System;
using System.Collections.Generic;

internal class Class8
{
    private const uint uint_0 = 5;

    private Class10 class10_0;

    private uint uint_1;

    public Class8(string string_0)
    {
        this.class10_0 = new Class10(string_0);
    }

    public KeyValuePair<uint, string> method_0()
    {
        this.class10_0.method_0();
        uint num = this.class10_0.method_1();
        if (num <= 5)
        {
            this.class10_0.method_3();
        }
        if (num >= 3)
        {
            this.class10_0.method_3();
        }
        if (num >= 4)
        {
            this.class10_0.method_5();
        }
        if (this.class10_0.method_3())
        {
            this.uint_1 = this.class10_0.method_1();
            this.class10_0.method_1();
            this.class10_0.method_1();
            this.class10_0.method_2();
        }
        if (num >= 2)
        {
            this.class10_0.method_3();
        }
        if (num >= 5)
        {
            this.class10_0.method_0();
        }
        uint num1 = this.class10_0.method_1();
        for (int i = 0; (long)i < (long)num1; i++)
        {
            this.class10_0.method_2();
        }
        num1 = this.class10_0.method_1();
        for (int j = 0; (long)j < (long)num1; j++)
        {
            Class9 class9 = new Class9(this.class10_0);
            if (class9.UInt32_0 == this.uint_1)
            {
                return new KeyValuePair<uint, string>(this.uint_1, class9.String_0);
            }
        }
        return new KeyValuePair<uint, string>(0, null);
    }
}