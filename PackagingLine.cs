// Linie de ambalare (cutii de carton) – fabrica TM.
// Scrisă când fabrica avea o singură linie: „oricum e una singură, fac totul static".
// Ulterior cineva a adăugat un constructor, ca să „arate a obiect".
//
// Modelul PL-120: viteză maximă 120 pachete/min, putere 2 kW + 0,05 kW pentru fiecare pachet/min.
public class PackagingLine
{
  public const string PlantCode = "TM";             // codul fabricii
  public static double EnergyTariff { get; set; }          // lei/kWh – „nu se schimbă"
  private int MaxSpeed;                  // pachete/min (modelul PL-120)
  private readonly string model;                     // modelul liniei
  private readonly string name = "";
  private int speed;                         // pachete/min
  private int packedCount=0;
  private double energyKwh;
  private string currentBatch = "";
  private static int nextBatchNumber = 1;
  private static int linesCreated;
  private static int totalPackedCount;

  public PackagingLine(string lineName, string model)
  {
    this.model = model;
    name = lineName;
    linesCreated++;
    MaxSpeed = model=="PL-120" ? 120 : 80; // alte modele au viteza maxima 100 pachete/min
    EnergyTariff = ReadTariff("plant.config");
  }
  double ReadTariff(string path)
  {
    foreach (string line in File.ReadAllLines(path))
    {
      if (line.StartsWith("EnergyTariff="))
        return double.Parse(line["EnergyTariff=".Length..], System.Globalization.CultureInfo.InvariantCulture);
    }
    throw new InvalidOperationException("EnergyTariff lipsește din " + path);
  }
  public string Name => name;
  public int Speed => speed;
  public int PackedCount => packedCount;
  public double EnergyKwh => energyKwh;
  public string CurrentBatch => currentBatch;

  public static int LinesCreated => linesCreated;
  public static int TotalPacked => totalPackedCount;

  public int SetSpeed(int packsPerMinute)
  {
    return speed = Math.Clamp(packsPerMinute, 0, MaxSpeed);
  }

  // Fiecare lot primește un cod unic în toată fabrica: TM-0001, TM-0002, ...
  public string StartBatch()
  {
    currentBatch = $"{PlantCode}-{nextBatchNumber:D4}";
    nextBatchNumber++;
    return currentBatch;
  }

  public void Run(int minutes)
  {
    packedCount += speed * minutes;
    double powerKw = 2 + 0.05 * speed;
    energyKwh += powerKw * minutes / 60.0;
    totalPackedCount += speed * minutes;
  }

  public double EnergyCost()
  {
    return energyKwh * EnergyTariff;
  }
}
