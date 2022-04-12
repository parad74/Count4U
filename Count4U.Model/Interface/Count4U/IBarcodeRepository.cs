using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Barcode объектам
    /// </summary>
    public interface IBarcodeRepository
	{
        /// <summary>
        /// Получение всех объектов Barcode
        /// </summary>
        /// <returns></returns>
		Barcodes GetBarcodes(string pathDB);

        /// <summary>
        /// Клонирование объекта по описательной части объекта Barcode
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        Barcode Clone(Barcode barcode);

        /// <summary>
        /// Удаление 
        /// </summary>
        /// <param name="barcode"></param>
        void Delete(Barcode barcode, string pathDB);

        /// <summary>
        /// Удаление баркода по ID продукта
        /// </summary>
        /// <param name="productID"></param>
		void DeleteByProductID(long productID, string pathDB);

        /// <summary>
        /// Удаление баркода по значению баркода
        /// </summary>
        /// <param name="barcodeValue"></param>
		void DeleteAllByBarcodeValue(string barcodeValue, string pathDB);

        /// <summary>
        /// Удаление баркода по ID баркода
        /// </summary>
        /// <param name="barcodeID"></param>
		void DeleteAllByBarcodeID(long barcodeID, string pathDB);

        /// <summary>
        /// Вставить Barcode объект в БД
        /// </summary>
        /// <param name="barcode"></param>
		void Insert(Barcode barcode, string pathDB);

        /// <summary>
        /// Вставить Barcode объект, связав его с продуктом
        /// </summary>
        /// <param name="barcode"></param>
		void Insert(Barcode barcode, Product product, string pathDB);

        /// <summary>
        /// Изменить Barcode объект
        /// </summary>
        /// <param name="barcode"></param>
		void Update(Barcode barcode, string pathDB);

        /// <summary>
        /// Проверяет существует ли Barcode, с заданным значением 
        /// </summary>
        /// <param name="barcodeValue"></param>
        /// <returns></returns>
        bool IsExistsBarcode(string barcodeValue, string pathDB);

        /// <summary>
        /// Проверяет существует ли Barcode, с заданным значением с заданным продуктом TO DO ??
        /// </summary>
        /// <param name="barcodeValue"></param>
        /// <param name="product"></param>
        /// <returns></returns>
		bool IsExistsBarcode(string barcodeValue, Product product, string pathDB); //??

	}
}
