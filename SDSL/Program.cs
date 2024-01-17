using SDSL;

// See https://aka.ms/new-console-template for more information
try
{
    SDSLReader reader = new SDSLReader("D:\\Dev\\Projetos\\SDSL\\SDSL\\SDSL\\data\\TestData.sdsl");
    Console.WriteLine(reader.Data["name"]);
    Console.WriteLine(reader.Data["age"]);
    Console.WriteLine(reader.Data["wife"]["name"]);
    Console.WriteLine(reader.Data["wife"]["age"]);

    reader.Close();
}catch(Exception err)
{
    Console.WriteLine(err.ToString());
}


Console.Read();

