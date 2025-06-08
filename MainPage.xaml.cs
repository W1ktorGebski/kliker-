namespace MauiApp4;

public partial class MainPage : ContentPage
{
    int wynik = 0;
    int czas = 30;
    System.Timers.Timer? licznik;
    Random los = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void KliknijStart(object sender, EventArgs e)
    {
        wynik = 0;
        czas = 30;

        LabelWynik.Text = "Wynik: 0";
        LabelCzas.Text = "Czas: 30";

        PrzyciskStart.IsVisible = false;
        KwadratCel.IsVisible = true;

        int próby = 0;
        while (!CzyLayoutGotowy() && próby++ < 10)
        {
            await Task.Delay(100);
        }

        PrzesuńKwadrat();

        licznik = new System.Timers.Timer(1000);
        licznik.Elapsed += OdliczajCzas;
        licznik.Start();
    }

    private void OdliczajCzas(object sender, System.Timers.ElapsedEventArgs e)
    {
        czas--;

        Dispatcher.Dispatch(() =>
        {
            LabelCzas.Text = $"Czas: {czas}";

            if (czas <= 0)
            {
                licznik?.Stop();
                KwadratCel.IsVisible = false;
                PrzyciskStart.IsVisible = true;

                DisplayAlert("Koniec!", $"Twój wynik: {wynik}", "Zagraj jeszcze raz")
                    .ContinueWith(_ =>
                    {
                        Dispatcher.Dispatch(() =>
                        {
                            LabelWynik.Text = "Wynik: 0";
                            LabelCzas.Text = "Czas: 30";
                        });
                    });
            }
        });
    }

    private void KliknijKwadrat(object sender, EventArgs e)
    {
        wynik++;
        LabelWynik.Text = $"Wynik: {wynik}";
        PrzesuńKwadrat();
    }

    private void PrzesuńKwadrat()
    {
        double szerokośćKwadratu = KwadratCel.Width;
        double wysokośćKwadratu = KwadratCel.Height;

        double maxX = LayoutGłówny.Width - szerokośćKwadratu;
        double maxY = LayoutGłówny.Height - wysokośćKwadratu;

        if (maxX <= 0 || maxY <= 0)
            return;

        Rect obszarWyniku = new Rect(LabelWynik.X, LabelWynik.Y, LabelWynik.Width, LabelWynik.Height);
        Rect obszarCzasu = new Rect(LabelCzas.X, LabelCzas.Y, LabelCzas.Width, LabelCzas.Height);

        int próby = 0;
        while (próby++ < 100)
        {
            double x = los.NextDouble() * maxX;
            double y = los.NextDouble() * maxY;
            Rect nowyKwadrat = new Rect(x, y, szerokośćKwadratu, wysokośćKwadratu);

            if (!nowyKwadrat.IntersectsWith(obszarWyniku) && !nowyKwadrat.IntersectsWith(obszarCzasu))
            {
                AbsoluteLayout.SetLayoutBounds(KwadratCel, nowyKwadrat);
                return;
            }
        }
    }

    private bool CzyLayoutGotowy()
    {
        return LayoutGłówny.Width > 0 && LayoutGłówny.Height > 0;
    }
}
