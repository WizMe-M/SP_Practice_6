# "Пункт наблюдения"
Приложение для работы с Arduino через подключение к COM-порту

Приложение состоит из двух частей:
1) Оконное приложение WPF
2) Прошивка для Arduino Multifunctional Shield

В WPF-клиенте пользователь имеет следующий интерфейс.

![Интерфейс WPF-клиента](https://github.com/WizMe-M/SP_Practice_6/raw/master/Practice_6/interface.png)
- Выпадающий список отображает доступные для подключения COM-порты;
- Кнопка "Найти" обновляет выпадающий список
- Кнопка "Подключиться" - при нажатии производится подключение/отключение от COM-порта. При нажатии цвет и надпись на кнопке и меняются, на зеленый ("Arduino подключен") и красный ("Arduino отключен")
- Кнопки "Включить/Выключить подстанцию №1/№2" - соответственно зажигает и тушит D1 и D2 светодиоды на плате
- Кнопка "Отключить сигнализацию" - отключает на плате спикер S1
 
На плате Arduino считываются нажатия на кнопки A1, A2 и A3 и чтение полученных данных с COM-порта. Если происходит нажатие на какую-либо из кнопок, на WPF-клиент отправляется сообщение о вторжении с предложением включить сигнализацию. Если согласиться, то на плате включается спикер S1.
