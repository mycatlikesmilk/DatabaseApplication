using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestApp
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine(@"
Доступные команды:
addGood <title> - добавляет продукт в базу данных
deleteGood <id> - удаляет продукт из базы данных
addPharmacy <title> <address> <phone> - добавляет аптеку в базу данных. Телефон вводить без 8 или +7
deletePharmacy <id> - удаляет аптеку из базы данных
addStorage <pharmacyID> <title> - добавляет склад, привязанный к указанной аптеке, в базу данных
deleteStorage <id> - удаляет склад из базы данных
addButch <goodID> <storageID> <amount> - добавляет партию в базу данных
deleteButch <id> - удаляет партию из базы данных
showGoods <pharmacyID> - показывает список всех товаров в указанной аптеке
");
            while (true)
            {
                ParseCommand(Console.ReadLine(), out string command, out string[] args);
                switch (command)
                {
                    case "addGood":
                        CreateGood(args[0]);
                        break;
                    case "deleteGood":
                        DeleteGood(args[0]);
                        break;
                    case "addPharmacy":
                        CreatePharmacy(args[0], args[1], args[2]);
                        break;
                    case "deletePharmacy":
                        DeletePharmacy(args[0]);
                        break;
                    case "addStorage":
                        CreateStorage(args[0], args[1]);
                        break;
                    case "deleteStorage":
                        DeleteStorage(args[0]);
                        break;
                    case "addButch":
                        CreateButch(args[0], args[1], args[2]);
                        break;
                    case "deleteButch":
                        DeleteButch(args[0]);
                        break;
                    case "showGoods":
                        ShowAllGoods(args[0]);
                        break;
                }
            }
        }

        #region Commands
        private static void CreateGood(string title)
        {
            try
            {
                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var newGood = new good()
                    {
                        title = title
                    };
                    context.goods.InsertOnSubmit(newGood);
                    context.SubmitChanges();
                    Console.WriteLine($"{title} добавлен");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время создания товара: {e.Message}");
            }
        }

        private static void DeleteGood(string id)
        {
            try
            {
                if (!int.TryParse(id, out int goodId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }

                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var good = context.goods.FirstOrDefault(x => x.good_id == goodId);

                    if (good == null)
                        Console.WriteLine($"Товара с ID {id} в базе данных не найдено");
                    else
                    {
                        context.goods.DeleteOnSubmit(good);
                        context.SubmitChanges();
                        Console.WriteLine($"{good.title} удален");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время удаления товара: {e.Message}");
            }
        }

        private static void CreatePharmacy(string title, string address, string phone)
        {
            try
            {
                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var newPharmacy = new pharmacy()
                    {
                        title = title,
                        addr = address,
                        phone = phone
                    };
                    context.pharmacies.InsertOnSubmit(newPharmacy);
                    context.SubmitChanges();
                    Console.WriteLine($"{title} добавлена");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время создания аптеки: {e.Message}");
            }
        }

        private static void DeletePharmacy(string id)
        {
            try
            {
                if (!int.TryParse(id, out int pharmacyId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }

                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var pharmacy = context.pharmacies.FirstOrDefault(x => x.pharmacy_id == pharmacyId);
                    if (pharmacy == null)
                        Console.WriteLine($"Аптеки с ID {id} в базе данных не найдено");
                    else
                    {
                        context.pharmacies.DeleteOnSubmit(pharmacy);
                        context.SubmitChanges();
                        Console.WriteLine($"{pharmacy.title} удалена");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время удаления аптеки: {e.Message}");
            }
        }

        private static void CreateStorage(string pharmacyId, string title)
        {
            try
            {
                if (!int.TryParse(pharmacyId, out int PharmacyId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }

                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var newStorage = new storage()
                    {
                        pharmacy_id = PharmacyId,
                        title = title
                    };
                    context.storages.InsertOnSubmit(newStorage);
                    context.SubmitChanges();
                    Console.WriteLine($"{title} добавлен");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время создания склада: {e.Message}");
            }
        }

        private static void DeleteStorage(string id)
        {
            try
            {
                if (!int.TryParse(id, out int storageId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }

                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var storage = context.storages.FirstOrDefault(x => x.storage_id == storageId);
                    if (storage == null)
                        Console.WriteLine($"Склада с ID {id} в базе данных не найдено");
                    else
                    {
                        context.storages.DeleteOnSubmit(storage);
                        context.SubmitChanges();
                        Console.WriteLine($"{storage.title} удален");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время удаления склада: {e.Message}");
            }
        }

        private static void CreateButch(string goodId, string storageId, string amount)
        {
            try
            {
                if (!int.TryParse(goodId, out int GoodId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }
                if (!int.TryParse(storageId, out int StorageId))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }
                if (!int.TryParse(amount, out int Amount))
                {
                    Console.WriteLine("Неверный ввод данных");
                    return;
                }

                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var newButch = new butch()
                    {
                        good_id = GoodId,
                        storage_id = StorageId,
                        amount = Amount
                    };
                    context.butches.InsertOnSubmit(newButch);
                    context.SubmitChanges();
                    Console.WriteLine($"Поставка добавлена");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время создания поставки: {e.Message}");
            }
        }

        private static void DeleteButch(string id)
        {
            try
            {
                int butchId = int.Parse(id);
                using (PharmaciesDataContext context = new PharmaciesDataContext())
                {
                    var butch = context.butches.FirstOrDefault(x => x.butch_id == butchId);
                    if (butch == null)
                        Console.WriteLine($"Партии с ID {id} в базе данных не найдено");
                    else
                    {
                        context.butches.DeleteOnSubmit(butch);
                        context.SubmitChanges();
                        Console.WriteLine($"Поставка удалена");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка во время удаления поставки: {e.Message}");
            }
        }

        private static void ShowAllGoods(string pharmacyId)
        {
            int PharmacyId = int.Parse(pharmacyId);
            using (PharmaciesDataContext context = new PharmaciesDataContext())
            {
                var goods =
                    (from b in context.butches
                     join s in context.storages on b.storage_id equals s.storage_id into gs
                     from ts in gs.DefaultIfEmpty()
                     join p in context.pharmacies on ts.pharmacy_id equals p.pharmacy_id into gp
                     from tp in gp.DefaultIfEmpty()
                     join g in context.goods on b.good_id equals g.good_id into gg
                     from tg in gg.DefaultIfEmpty()
                     where tp.pharmacy_id == PharmacyId
                     group b.amount by tg.title into grouped
                     select new
                     {
                         Product = grouped.Key,
                         Amount = grouped.Sum()
                     }).ToList();
                foreach (var i in goods)
                {
                    Console.WriteLine($"{i.Product}:\t{i.Amount}");
                }
            }
        }
        #endregion

        #region Utils
        private static void ParseCommand(string line, out string command, out string[] arguments)
        {
            Regex ex = new Regex("(\\w+)|(\"[\\w]+\")");
            var matches = ex.Matches(line);
            List<string> args = new List<string>();
            command = matches[0].Value;
            for (int i = 1; i < matches.Count; i++)
            {
                args.Add(matches[i].Value);
            }
            arguments = args.ToArray();
        }
        #endregion
    }
}
