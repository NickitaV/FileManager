using System;
using System.IO;


namespace File_Manager
{
    class Program
    {
        static void Main(string[] args)
        {

            bool ExitControl = true;
            /*string NewDir = @"e:/Games";*/
            Console.WriteLine("Введите название директории");
            string NewDir = @Console.ReadLine();
            while (ExitControl == true)
            {

                string[] Dir = FoundDir(NewDir);
                GoDir(Dir, out ConsoleKeyInfo key, out int c);
                if (key.Key == ConsoleKey.Backspace) { Console.Clear(); ExitControl = false; }
                NewDir = Dir[c];


                if (key.Key == ConsoleKey.Enter)
                {
                    СhoiceDir(Dir, c, out string N);

                    NewDir = N;
                }




            }


        }
        static string[] FoundDir(string NewDir)
        {

            string[] Arr = Directory.GetDirectories(NewDir);

            string[] DirArr = new string[Arr.Length + 1];
            for (int n = 0; n < Arr.Length; n++) { DirArr[n + 1] = Arr[n]; }

            DirArr[0] = "......";

            return DirArr;
        }

        static int ColorChoice(int i, int c, string[] DirArr)
        {

            if (i != c)
            {
                Console.WriteLine(DirArr[i]);

            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(DirArr[c]);
                Console.ResetColor();
            }
            return c;
        }

        static void GoDir(string[] DirArr, out ConsoleKeyInfo key, out int c)
        {
            c = 0;
            int Index = 0;
            int page = 0;
            int Fpage = 0;
            do
            {


                Console.WriteLine("[Tab] - стр. файлов, [Стрелки вправ, влево] - стр. директории, [Space] - пред./след. файл, \n[Стрелки вверх, вниз] - след./пред. директория, [Escape] - ввод команды, [Enter] - выбор директории");
                Console.WriteLine("============================== Directories ============================== [Backspace] - выход");
                for (int i = page * 20; (i < (20 + (page * 20))) && (i < DirArr.Length); i++)
                {
                    c = ColorChoice(i, c, DirArr);

                }
                string[] FArr = Directory.GetFiles(DirArr[c]);
                Console.WriteLine("========Files======[Esc] - ввести команду([cp]-копировать файл/дир.[rm]-удалить файл/дир.[info]-инфо");

                for (int i = Fpage * 8; (i < (8 + (Fpage * 8))) && (i < FArr.Length); i++) { Index = ColorChoice(i, Index, FArr); }


                key = Console.ReadKey();

                if (key.Key == ConsoleKey.Tab)
                {


                    Console.Clear(); if ((Fpage * 8) < FArr.Length) { Fpage++; Index = Index + 8 * Fpage; } else { Fpage = 0; }
                }
                if (key.Key == ConsoleKey.Spacebar)
                {
                    Console.Clear();

                    Index++;
                    if ((Index > (8 * Fpage + 7)) || (Index > (FArr.Length - 1))) { Index = Fpage * 8; }
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    c = c + 20;
                    page++; if ((page * 20) > DirArr.Length) { page = 0; c = 0; }

                    Console.Clear();

                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    c = c - 20;
                    page--; if (page < 0) { page = 0; c = 0; }

                    Console.Clear();
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    Console.Clear();
                    c++;
                    if ((c > 20 * page + 19) || (c > DirArr.Length - 1)) { c = page * 20; }

                }
                if (key.Key == ConsoleKey.UpArrow)
                {
                    Console.Clear();
                    c--; if (c < page * 20) { if ((page * 20 + 20) > (DirArr.Length - 1)) { c = (DirArr.Length - 1); } else { c = page * 20 + 19; } }

                }
                if (key.Key == ConsoleKey.Escape)
                {

                    Console.WriteLine("==================Введите команду:");
                    string command = Console.ReadLine();
                    switch (command)
                    {

                        case "cp":
                            Console.WriteLine("Копировать файл или директорию?(f-если файл, d-если директорию)");
                            string Str = Console.ReadLine();
                            if (((Str != "f") && (Str != "d")) || (string.IsNullOrEmpty(Str))) { Console.WriteLine("Введите f или d"); break; }
                            else
                            {

                                if (Str == "f")
                                {
                                    try
                                    {
                                        string from = @$"{FArr[Index]}";
                                        Console.WriteLine($"Сейчас файл в директории {DirArr[c]}.\nВведите новую директорию для файла {Path.GetFileName(FArr[Index])}");
                                        string readAdress = Console.ReadLine(); string newAdress = @$"{readAdress}/{Path.GetFileName(FArr[Index])}";
                                        File.Copy(from, newAdress, true); break;
                                    }
                                    catch (FileNotFoundException) { }
                                    catch (UnauthorizedAccessException) { }
                                }
                                if (Str == "d")
                                {
                                    try
                                    {
                                        Console.WriteLine($"Введите новую директорию для копирования{DirArr[c]}");
                                        string oldAdresDir = DirArr[c];
                                        string newAdressDir = Console.ReadLine();
                                        static void RecursCopyDir(string oldAdresDir, string newAdressDir)
                                        {

                                            DirectoryInfo CopyDir = new DirectoryInfo(oldAdresDir);
                                            foreach (DirectoryInfo copydir in CopyDir.GetDirectories())
                                            {
                                                if (Directory.Exists($"{ newAdressDir}/{copydir.Name}") != true)
                                                {
                                                    Directory.CreateDirectory($"{ newAdressDir}/{copydir.Name}");
                                                }
                                                RecursCopyDir(copydir.FullName, $"{ newAdressDir}/{copydir.Name}");
                                            }
                                            foreach (string copyfile in Directory.GetFiles(oldAdresDir))
                                            {
                                                string newNewAdress = @$"{newAdressDir}/{Path.GetFileName(copyfile)}";
                                                File.Copy(copyfile, newNewAdress, true);
                                            }
                                        }

                                        RecursCopyDir(oldAdresDir, newAdressDir);
                                    }
                                    catch (DirectoryNotFoundException) { }
                                    catch (UnauthorizedAccessException) { }
                                }

                                break;
                            }


                        case "rm":
                            Console.WriteLine("Удалить файл или директорию?(f-если файл, d-если директорию)");
                            string StrRm = Console.ReadLine();
                            if (((StrRm != "f") && (StrRm != "d")) || (string.IsNullOrEmpty(StrRm))) { Console.WriteLine("Введите f или d"); break; }
                            else
                            {
                                if (StrRm == "f")
                                {
                                    try
                                    {
                                        File.Delete(FArr[Index]); Console.WriteLine($"Файл {Path.GetFileName(FArr[Index])} удалён"); break;
                                    }
                                    catch (FileNotFoundException) { }
                                    catch (UnauthorizedAccessException) { }
                                }


                                if (StrRm == "d")
                                {
                                    try
                                    {
                                        string thisAdresDir = DirArr[c];
                                        static void DeleteDir(string thisAdresDir)
                                        {

                                            DirectoryInfo DelDir = new DirectoryInfo(thisAdresDir);
                                            foreach (DirectoryInfo DelFolder in DelDir.GetDirectories())
                                            {
                                                DeleteDir(DelFolder.FullName);
                                                if ((DelFolder.GetDirectories().Length == 0) && (DelFolder.GetFiles().Length == 0))
                                                {
                                                    DelFolder.Delete();
                                                }

                                            }
                                            foreach (string Dfile in Directory.GetFiles(thisAdresDir))
                                            {

                                                File.Delete(Dfile);
                                            }

                                        }
                                        DeleteDir(thisAdresDir); Directory.Delete(DirArr[c], true);
                                        Console.Clear();
                                        for (int j = 0; j < DirArr.Length; j++) { if ((j >= c) && (j < DirArr.Length - 1)) { DirArr[j] = DirArr[j + 1]; } }
                                    }
                                    catch (DirectoryNotFoundException) { }
                                    catch (UnauthorizedAccessException) { }
                                    Console.WriteLine($"Папка {DirArr[c]} удалена полностью");
                                }
                                break;
                            }
                        case "info":
                            Console.WriteLine("Инфо о файле или директории?(f-если о файле, d-если директории)");
                            string StrI = Console.ReadLine();
                            if (((StrI != "f") && (StrI != "d")) || (string.IsNullOrEmpty(StrI))) { Console.WriteLine("Введите f или d"); break; }
                            else
                            {
                                if (StrI == "f")
                                {
                                    FileInfo fileI = new FileInfo(FArr[Index]);
                                    Console.WriteLine($"Имя файла: {fileI.Name} ");
                                    Console.WriteLine($"Время создания: {fileI.CreationTime}");
                                    Console.WriteLine($"Размер:{fileI.Length}");
                                    Console.WriteLine("Нажмите ENTER для продолжения:"); Console.ReadKey(); Console.Clear(); break;
                                }
                                if (StrI == "d")
                                {
                                    DirectoryInfo dirI = new DirectoryInfo(DirArr[c]);
                                    Console.WriteLine($"Название каталога: {dirI.Name}");
                                    Console.WriteLine($"Полное название каталога: {dirI.FullName}");
                                    Console.WriteLine($"Время создания каталога: {dirI.CreationTime}");
                                    Console.WriteLine($"Корневой каталог: {dirI.Root}");
                                    Console.WriteLine("Нажмите ENTER для продолжения:"); Console.ReadKey(); Console.Clear();
                                }
                                break;
                            }

                    }
                }


            } while ((key.Key != ConsoleKey.Enter) && (key.Key != ConsoleKey.Backspace));









        }
        static void СhoiceDir(string[] Dir, int c, out string N)
        {
            DirectoryInfo Dc = new DirectoryInfo(Dir[c]);


            if (Dir[c] == "......")
            {

                Console.Clear();


                FileInfo fInfo1 = new FileInfo(Dir[c + 1]);

                string Pdir = fInfo1.DirectoryName;
                FileInfo fInfo2 = new FileInfo(Pdir);
                string ParentDir = fInfo2.DirectoryName;





                N = @$"{ParentDir}";



            }
            else
            {
                if (Dc.GetDirectories().Length == 0)
                {
                    FileInfo fInfo = new FileInfo(Dir[c]);

                    string dir = fInfo.DirectoryName; Console.Clear(); N = @$"{dir}";
                    Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("В этой директории нет директорий!"); Console.ResetColor();
                }
                else { Console.Clear(); N = Dir[c]; }
            }







        }
    }
}