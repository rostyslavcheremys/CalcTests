using System;
using BaseCalculator;

class TestDriver
{
    const string OK = "";
    const string ERR_RANGE = "Error 06";
    const string ERR_DIV0 = "Error 09";
    
    const long MIN = int.MinValue, MAX = int.MaxValue;

    static int total = 0, passed = 0;

    static void Case(string method, int n, string req, Func<int> call, long a, long? b, int expRes, string expErr)
    {
        total++;
        string input = b.HasValue ? $"a = {a}, b = {b}" : $"a = {a}";
        Console.WriteLine($"[{method}] Test Case {n} ({req})");
        Console.WriteLine($"  Вхідні дані: {input}");
        Console.WriteLine($"  Очікується: res = {expRes}, error = \"{expErr}\"");
        int res = 0; Exception exc = null;
        var th = new System.Threading.Thread(() => { try { res = call(); } catch (Exception e) { exc = e; } });
        th.IsBackground = true;
        th.Start();
        if (!th.Join(3000))
        {
            Console.WriteLine("  ЗАВИСАННЯ: метод не завершився за 3 секунди\n  Тест НЕ пройдено\n");
            return;
        }
        if (exc != null)
        {
            Console.WriteLine($"  Перехоплено виняток: {exc.GetType().Name}: {exc.Message}\n  Тест НЕ пройдено\n");
            return;
        }
        string err = CalcClass.lastError;
        Console.WriteLine($"  Отримано:   res = {res}, error = \"{err}\"");
        bool ok = res == expRes && err == expErr;
        if (ok) passed++;
        Console.WriteLine(ok ? "  Тест пройдено\n" : "  Тест НЕ пройдено\n");
    }

    class Tee : System.IO.TextWriter
    {
        readonly System.IO.TextWriter a, b;
        public Tee(System.IO.TextWriter a, System.IO.TextWriter b) { this.a = a; this.b = b; }
        public override System.Text.Encoding Encoding { get { return a.Encoding; } }
        public override void Write(char c) { a.Write(c); b.Write(c); b.Flush(); }
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var file = new System.IO.StreamWriter("results.txt", false, new System.Text.UTF8Encoding(true));
        Console.SetOut(new Tee(Console.Out, file));
        // ---------- Mod ----------
        string m = "Mod";
        Case(m, 1, "T1: обидва в області", () => CalcClass.Mod(17, 5), 17, 5, 2, OK);
        Case(m, 2, "T1: від'ємне ділене", () => CalcClass.Mod(-17, 5), -17, 5, -2, OK);
        Case(m, 3, "T1: від'ємний дільник", () => CalcClass.Mod(17, -5), 17, -5, 2, OK);
        Case(m, 4, "T1: ділене = 0", () => CalcClass.Mod(0, 5), 0, 5, 0, OK);
        Case(m, 5, "T1: a < b", () => CalcClass.Mod(3, 7), 3, 7, 3, OK);
        Case(m, 6, "T1: межа, MIN % -1", () => CalcClass.Mod(MIN, -1), MIN, -1, 0, OK);
        Case(m, 7, "T2: b = 0", () => CalcClass.Mod(10, 0), 10, 0, 0, ERR_DIV0);
        Case(m, 8, "T2: b поза областю", () => CalcClass.Mod(10, 3000000000), 10, 3000000000, 0, ERR_RANGE);
        Case(m, 9, "T3: a поза областю", () => CalcClass.Mod(-2850800078, 7), -2850800078, 7, 0, ERR_RANGE);

        // ---------- ABS (унарний плюс) ----------
        m = "ABS";
        Case(m, 1, "T1: додатне", () => CalcClass.ABS(5), 5, null, 5, OK);
        Case(m, 2, "T1: від'ємне", () => CalcClass.ABS(-5), -5, null, -5, OK);
        Case(m, 3, "T1: нуль", () => CalcClass.ABS(0), 0, null, 0, OK);
        Case(m, 4, "T1: верхня межа", () => CalcClass.ABS(MAX), MAX, null, (int)MAX, OK);
        Case(m, 5, "T1: нижня межа", () => CalcClass.ABS(MIN), MIN, null, (int)MIN, OK);
        Case(m, 6, "T3: MAX+1", () => CalcClass.ABS(3000000000), 3000000000, null, 0, ERR_RANGE);
        Case(m, 7, "T3: MIN-1", () => CalcClass.ABS(-2850800078), -2850800078, null, 0, ERR_RANGE);

        // ---------- IABS (унарний мінус) ----------
        m = "IABS";
        Case(m, 1, "T1: додатне", () => CalcClass.IABS(5), 5, null, -5, OK);
        Case(m, 2, "T1: від'ємне", () => CalcClass.IABS(-5), -5, null, 5, OK);
        Case(m, 3, "T1: нуль", () => CalcClass.IABS(0), 0, null, 0, OK);
        Case(m, 4, "T1: MAX", () => CalcClass.IABS(MAX), MAX, null, -(int)MAX, OK);
        Case(m, 5, "T4: -MIN поза областю", () => CalcClass.IABS(MIN), MIN, null, 0, ERR_RANGE);
        Case(m, 6, "T3: вхід поза областю", () => CalcClass.IABS(3000000000), 3000000000, null, 0, ERR_RANGE);

        // ---------- Sub ----------
        m = "Sub";
        Case(m, 1, "T1: звичайний", () => CalcClass.Sub(78508, 304), 78508, 304, 78204, OK);
        Case(m, 2, "T1: від'ємний результат", () => CalcClass.Sub(304, 78508), 304, 78508, -78204, OK);
        Case(m, 3, "T1: нулі", () => CalcClass.Sub(0, 0), 0, 0, 0, OK);
        Case(m, 4, "T1: межа MAX - 0", () => CalcClass.Sub(MAX, 0), MAX, 0, (int)MAX, OK);
        Case(m, 5, "T1: межа MIN - 0", () => CalcClass.Sub(MIN, 0), MIN, 0, (int)MIN, OK);
        Case(m, 6, "T2: b поза областю", () => CalcClass.Sub(1, 3000000000), 1, 3000000000, 0, ERR_RANGE);
        Case(m, 7, "T3: a поза областю", () => CalcClass.Sub(-2850800078, 1), -2850800078, 1, 0, ERR_RANGE);
        Case(m, 8, "T4: переповнення вгору", () => CalcClass.Sub(2000000000, -2000000000), 2000000000, -2000000000, 0, ERR_RANGE);
        Case(m, 9, "T4: переповнення вниз", () => CalcClass.Sub(MIN, 1), MIN, 1, 0, ERR_RANGE);

        // ---------- Mult ----------
        m = "Mult";
        Case(m, 1, "T1: звичайний", () => CalcClass.Mult(123, -45), 123, -45, -5535, OK);
        Case(m, 2, "T1: множення на 0", () => CalcClass.Mult(MAX, 0), MAX, 0, 0, OK);
        Case(m, 3, "T1: множення на 1", () => CalcClass.Mult(MIN, 1), MIN, 1, (int)MIN, OK);
        Case(m, 4, "T1: максимум без переповнення", () => CalcClass.Mult(46340, 46340), 46340, 46340, 2147395600, OK);
        Case(m, 5, "T2: b поза областю", () => CalcClass.Mult(2, 3000000000), 2, 3000000000, 0, ERR_RANGE);
        Case(m, 6, "T3: a поза областю", () => CalcClass.Mult(-2850800078, 2), -2850800078, 2, 0, ERR_RANGE);
        Case(m, 7, "T4: 46341*46341", () => CalcClass.Mult(46341, 46341), 46341, 46341, 0, ERR_RANGE);
        Case(m, 8, "T4: MIN * -1", () => CalcClass.Mult(MIN, -1), MIN, -1, 0, ERR_RANGE);
        Case(m, 9, "T4: 100000*100000", () => CalcClass.Mult(100000, 100000), 100000, 100000, 0, ERR_RANGE);

        // ---------- Div ----------
        m = "Div";
        Case(m, 1, "T1: звичайний (усічення)", () => CalcClass.Div(78508, -304), 78508, -304, -258, OK);
        Case(m, 2, "T1: -7 / 2", () => CalcClass.Div(-7, 2), -7, 2, -3, OK);
        Case(m, 3, "T1: 0 / 5", () => CalcClass.Div(0, 5), 0, 5, 0, OK);
        Case(m, 4, "T1: a < b", () => CalcClass.Div(3, 7), 3, 7, 0, OK);
        Case(m, 5, "T1: MIN / 1", () => CalcClass.Div(MIN, 1), MIN, 1, (int)MIN, OK);
        Case(m, 6, "T2: b = 0", () => CalcClass.Div(10, 0), 10, 0, 0, ERR_DIV0);
        Case(m, 7, "T2: b поза областю", () => CalcClass.Div(10, 3000000000), 10, 3000000000, 0, ERR_RANGE);
        Case(m, 8, "T3: a поза областю", () => CalcClass.Div(-2850800078, 7), -2850800078, 7, 0, ERR_RANGE);
        Case(m, 9, "T4: MIN / -1", () => CalcClass.Div(MIN, -1), MIN, -1, 0, ERR_RANGE);

        Console.WriteLine($"ПІДСУМОК: пройдено {passed} з {total}");
        Console.WriteLine("Натисніть будь-яку клавішу...");
        Console.ReadKey();
    }
}
