using System;

internal class Class9
{
    private const uint uint_0 = 13;

    private string string_0;

    private uint uint_1;

    public string String_0
    {
        get
        {
            return this.string_0;
        }
        set
        {
            this.string_0 = value;
        }
    }

    public uint UInt32_0
    {
        get
        {
            return this.uint_1;
        }
        set
        {
            this.uint_1 = value;
        }
    }

    public Class9(Class10 class10_0)
    {
        uint num = class10_0.method_1();
        if (num >= 2 && num <= 13)
        {
            this.UInt32_0 = class10_0.method_1();
            if (num >= 3)
            {
                class10_0.method_1();
            }
            if (num >= 10)
            {
                class10_0.method_1();
            }
            int num1 = (num >= 5 ? 4 : 1);
            for (int i = 0; i < num1; i++)
            {
                uint num2 = class10_0.method_1();
                for (int j = 0; (long)j < (long)num2; j++)
                {
                    class10_0.method_5();
                    class10_0.method_1();
                    if (num >= 7)
                    {
                        class10_0.method_0();
                    }
                    if (num >= 11)
                    {
                        class10_0.method_5();
                    }
                    else if (num >= 9)
                    {
                        class10_0.method_5();
                    }
                }
            }
            if (num >= 6)
            {
                class10_0.method_3();
            }
            uint num3 = class10_0.method_1();
            if (num3 != 0)
            {
                this.String_0 = BitConverter.ToString(class10_0.method_4(num3)).Replace("-", "");
            }
            if (num >= 4)
            {
                class10_0.method_2();
            }
            else
            {
                class10_0.method_1();
            }
            if (num >= 8)
            {
                num3 = class10_0.method_1();
                if (num3 != 0)
                {
                    class10_0.method_4(num3);
                }
                class10_0.method_2();
            }
            if (num >= 12)
            {
                num3 = class10_0.method_1();
                if (num3 != 0)
                {
                    class10_0.method_4(num3);
                }
                class10_0.method_2();
            }
            class10_0.method_0();
            num3 = class10_0.method_1();
            for (int k = 0; (long)k < (long)num3; k++)
            {
                class10_0.method_0();
                class10_0.method_0();
                class10_0.method_2();
            }
            if (num >= 13)
            {
                num3 = class10_0.method_1();
                for (int l = 0; (long)l < (long)num3; l++)
                {
                    class10_0.method_0();
                    class10_0.method_0();
                    class10_0.method_2();
                }
            }
        }
    }
}