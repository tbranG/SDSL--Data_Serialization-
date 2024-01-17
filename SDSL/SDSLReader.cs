using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSL;

public class SDSLReader
{
    //sdsl source file
    private FileStream? source;
    private Dictionary<string, dynamic> data = new Dictionary<string, dynamic>();

    //Data getter
    public Dictionary<string, dynamic> Data { get => source is not null ? data : throw new InvalidOperationException("ERR: No file loaded"); }

    public bool IsClosed() => source is null;

    public SDSLReader(FileStream source)
    {
        this.source = source;
        try { BuildDataMap(); }
        catch(Exception err) 
        { 
            PrintErr(err.ToString());
            this.source.Close();
            this.source = null;
        }
    }

    public SDSLReader(string filePath)
    {
        try
        {
            source = File.OpenRead(filePath);
            BuildDataMap();
        }
        catch(Exception err)
        {
            PrintErr(err.ToString());
            source?.Close();
            source = null;
        }
    }

    //Closes the fileStream, blocking users from using this class methods
    public void Close() { source?.Close(); source = null; }

    //Loads all file content synchronously
    private void BuildDataMap()
    {
        if (source is null) return;

        // after reading a dollar sign, buildingKey is true, that means we must add every valid character after that
        // to the key string.
        SDSLStates parserState = SDSLStates.BUILDING_KEY;

        string _key = "";
        dynamic? _tempVal = null;

        Stack<string> parentKey = new();
        Stack<Dictionary<string, dynamic>> maps = new();

        maps.Push(data);
        do
        {
            int charCode = source.ReadByte();
            if (charCode == -1) break;
            char _c = (char)charCode;

            SDSLSymbols symb = CheckSymbol(_c);

            switch (symb)
            {
                case SDSLSymbols.DOLLAR_SIGN:
                    {
                        //if _c == $ we are reading a key
                        parserState = SDSLStates.BUILDING_KEY;
                        break;
                    }
                case SDSLSymbols.LETTER:
                    {
                        // in this case, the character is a letter, that means that we can be reading a key
                        // or a string value

                        if (parserState == SDSLStates.BUILDING_KEY)
                            _key += _c;
                        else if(parserState == SDSLStates.DEFINING_DATA_TYPE)
                        {     
                            maps.Peek().Add(_key, "");
                            _tempVal = "" + _c;

                            parserState = SDSLStates.READING_STRING;
                        }
                        else if(parserState == SDSLStates.READING_STRING)
                        {
                            _tempVal += _c;
                            maps.Peek()[_key] = _tempVal;
                        }
                        break;
                    }
                case SDSLSymbols.NUMBER:
                    {
                        if (parserState == SDSLStates.BUILDING_KEY) throw new SDSLInvalidKeyName("ERR: Attribute name cannot contains numbers");

                        // TODO: redo this logic, add support to float numbers and Date types
                        else if (parserState == SDSLStates.DEFINING_DATA_TYPE)
                        {
                            maps.Peek().Add(_key, 0);
                            _tempVal += _c;

                            parserState = SDSLStates.READING_NUMBER;
                        }
                        else if (parserState == SDSLStates.READING_NUMBER)
                        {
                            _tempVal += _c;
                            maps.Peek()[_key] = int.Parse(_tempVal);
                        }

                        break;
                    }
                case SDSLSymbols.SPACE:
                    {
                        if (parserState == SDSLStates.BUILDING_KEY) parserState = SDSLStates.DEFINING_DATA_TYPE;
                        break;
                    }
                case SDSLSymbols.END_LINE:
                    {
                        //reseting key and values
                        parserState = SDSLStates.SWITCHING_LINE;
                        _key = "";
                        _tempVal = "";

                        break;
                    }
                case SDSLSymbols.PARENTHESES:
                    {
                        // In this case we are creating an object attribute. The key value is mapping another dictionary
                        if (parserState == SDSLStates.BUILDING_KEY) throw new SDSLInvalidKeyName("ERR: Attribute name can only have letters and underlines");

                        if (parserState == SDSLStates.DEFINING_DATA_TYPE)
                        {
                            maps.Peek().Add(_key, new Dictionary<string, dynamic>());
                            maps.Push(maps.Peek()[_key]);
                            
                            parentKey?.Push(_key);

                            parserState = SDSLStates.BUILDING_OBJECT;
                        }
                        else if (parserState == SDSLStates.BUILDING_OBJECT || parserState == SDSLStates.SWITCHING_LINE)
                        {
                            maps.Pop();
                            parentKey?.Pop();
                            parserState = SDSLStates.SWITCHING_LINE;
                        }
                        
                        break;
                    }
                default: break;
            }
        } while (source.Position <= source.Length);
    }

    //Every single character from the file is checked
    private static SDSLSymbols CheckSymbol(char symbol) => symbol switch
    {
        '$' => SDSLSymbols.DOLLAR_SIGN,
        '[' or ']' => SDSLSymbols.BRACKETS,
        '(' or ')' => SDSLSymbols.PARENTHESES,
        '\n' => SDSLSymbols.END_LINE,
        ';' => SDSLSymbols.SEMICOLON,
        ' ' => SDSLSymbols.SPACE,
        _ => IntervalChecker(symbol)
    };

    private static SDSLSymbols IntervalChecker(char symbol)
    {
        int asciiCode = (int)symbol;
        if(asciiCode >= 48 && asciiCode <= 57)
        {
            return SDSLSymbols.NUMBER;
        }
        if((asciiCode >= 65 && asciiCode <= 90) || (asciiCode >= 97 && asciiCode <= 122))
        {
            return SDSLSymbols.LETTER;
        }

        return SDSLSymbols.TRASH;
    }
    //Console message display ----------------------------------------------
    private static void PrintErr(string? message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message ?? "ERR: Something went wrong");
        Console.ResetColor();
    }

    private static void PrintWarn(string? message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message ?? "WARNING: Undefined behavior");
        Console.ResetColor();
    }
}
