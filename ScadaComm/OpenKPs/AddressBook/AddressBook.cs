﻿/*
 * Copyright 2016 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : AddressBook
 * Summary  : Address book
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2015
 * Modified : 2015
 */

using System;
using System.Collections.Generic;
using System.Xml;

namespace Scada.Comm.Devices.AddressBook
{
    /// <summary>
    /// Address book
    /// <para>Адресная книга</para>
    /// </summary>
    public class AddressBook
    {
        /// <summary>
        /// Интерфейс, задающий свойство имени
        /// </summary>
        public interface INamedItem
        {
            /// <summary>
            /// Получить или установить имя
            /// </summary>
            string Name { get; set; }
        }

        /// <summary>
        /// Класс для сравнения объектов по имени
        /// </summary>
        public class ItemByNameComparer : IComparer<INamedItem>
        {
            /// <summary>
            /// Сравнить два объекта
            /// </summary>
            public int Compare(INamedItem x, INamedItem y)
            {
                string nameX = x == null ? null : x.Name;
                string nameY = y == null ? null : y.Name;
                return string.Compare(nameX, nameY, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Группа контактов
        /// </summary>
        public class ContactGroup : INamedItem
        {
            /// <summary>
            /// Конструктор
            /// </summary>
            public ContactGroup()
                : this("")
            {
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            public ContactGroup(string name)
            {
                Name = name ?? "";
                Contacts = new List<Contact>();
            }

            /// <summary>
            /// Получить или установить наименование группы
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Получить контакты, упорядоченные по имени
            /// </summary>
            public List<Contact> Contacts { get; private set; }
        }

        /// <summary>
        /// Контакт
        /// </summary>
        public class Contact : INamedItem
        {
            /// <summary>
            /// Конструктор
            /// </summary>
            public Contact()
                : this("")
            {
            }
            /// <summary>
            /// Конструктор
            /// </summary>
            public Contact(string name)
            {
                Name = name ?? "";
            }

            /// <summary>
            /// Получить или установить имя контакта
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Получить упорядоченные телефонные номера
            /// </summary>
            public List<string> PhoneNumbers { get; private set; }
            /// <summary>
            /// Получить упорядоченные адреса электронной почты
            /// </summary>
            public List<string> Emails { get; private set; }
        }


        /// <summary>
        /// Имя файла адресной книги по умолчанию
        /// </summary>
        public const string DefFileName = "AddressBook.xml";
        /// <summary>
        /// Объект для сравнения объектов по имени
        /// </summary>
        public static readonly ItemByNameComparer ByNameComparer = new ItemByNameComparer();


        /// <summary>
        /// Конструктор
        /// </summary>
        public AddressBook()
        {
            ContactGroups = new List<ContactGroup>();
        }


        /// <summary>
        /// Получить группы контактов, упорядоченные по наименованию
        /// </summary>
        public List<ContactGroup> ContactGroups { get; private set; }


        /// <summary>
        /// Загрузить адресную книгу из файла
        /// </summary>
        public bool Load(string fileName, out string errMsg)
        {
            try
            {
                // очистка адресной книги
                ContactGroups.Clear();

                // загрузка адресной книги
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);

                XmlNodeList contactGroupNodes = xmlDoc.DocumentElement.SelectNodes("ContactGroup");

                foreach (XmlElement contactGroupElem in contactGroupNodes)
                {
                    ContactGroup contactGroup = new ContactGroup(contactGroupElem.GetAttribute("name"));

                    XmlNodeList contactNodes = contactGroupElem.SelectNodes("Contact");
                    foreach (XmlElement contactElem in contactNodes)
                    {
                        Contact contact = new Contact(contactElem.GetAttribute("name"));

                        XmlNodeList phoneNumberNodes = contactGroupElem.SelectNodes("PhoneNumber");
                        foreach (XmlElement phoneNumberElem in phoneNumberNodes)
                            contact.PhoneNumbers.Add(phoneNumberElem.InnerText);
                        contact.PhoneNumbers.Sort();

                        XmlNodeList emailNodes = contactGroupElem.SelectNodes("Email");
                        foreach (XmlElement emailElem in emailNodes)
                            contact.Emails.Add(emailElem.InnerText);
                        contact.Emails.Sort();

                        contactGroup.Contacts.Add(contact);
                    }

                    contactGroup.Contacts.Sort(ByNameComparer);
                    ContactGroups.Add(contactGroup);
                }

                ContactGroups.Sort(ByNameComparer);

                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = CommPhrases.LoadKpDllSettingsError + ":" + Environment.NewLine + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Сохранить адресную книгу в файле
        /// </summary>
        public bool Save(string fileName, out string errMsg)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlElement rootElem = xmlDoc.CreateElement("AddressBook");
                xmlDoc.AppendChild(rootElem);

                foreach (ContactGroup contactGroup in ContactGroups)
                {
                    XmlElement contactGroupElem = xmlDoc.CreateElement("ContactGroup");
                    contactGroupElem.SetAttribute("name", contactGroup.Name);
                    rootElem.AppendChild(contactGroupElem);

                    foreach (Contact contact in contactGroup.Contacts)
                    {
                        XmlElement contactElem = xmlDoc.CreateElement("Contact");
                        contactElem.SetAttribute("name", contact.Name);

                        foreach (string phoneNumber in contact.PhoneNumbers)
                            contactElem.AppendElem("PhoneNumber", phoneNumber);

                        foreach (string email in contact.Emails)
                            contactElem.AppendElem("Email", email);

                        contactGroupElem.AppendChild(contactElem);
                    }
                }

                xmlDoc.Save(fileName);
                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = CommPhrases.SaveKpDllSettingsError + ":" + Environment.NewLine + ex.Message;
                return false;
            }
        }
    }
}
