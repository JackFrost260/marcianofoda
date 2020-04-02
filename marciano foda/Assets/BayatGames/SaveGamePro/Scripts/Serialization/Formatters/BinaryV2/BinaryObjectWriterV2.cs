using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;

using GameObject = UnityEngine.GameObject;
using Transform = UnityEngine.Transform;
using UnityObject = UnityEngine.Object;
using UnityComponent = UnityEngine.Component;

using BayatGames.SaveGamePro.Reflection;
using BayatGames.SaveGamePro.Serialization.Types;
using BayatGames.SaveGamePro.Utilities;

namespace BayatGames.SaveGamePro.Serialization.Formatters.BinaryV2
{

#if BAYAT_DEVELOPER

    /// <summary>
    /// Binary object writer.
    /// </summary>
    public class BinaryObjectWriterV2 : IDisposable, ISaveGameWriter
    {

    #region Fields

        /// <summary>
        /// The writer.
        /// </summary>
        protected BinaryWriter writer;

        /// <summary>
        /// The settings.
        /// </summary>
        protected SaveGameSettings settings;

    #endregion

    #region Properties

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        public virtual BinaryWriter Writer
        {
            get
            {
                return this.writer;
            }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public virtual SaveGameSettings Settings
        {
            get
            {
                return this.settings;
            }
            set
            {
                this.settings = value;
            }
        }

    #endregion

    #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BayatGames.SaveGamePro.Serialization.Formatters.Binary.BinaryObjectWriter"/> class.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public BinaryObjectWriterV2(Stream stream, SaveGameSettings settings) : this(new BinaryWriter(
                stream,
                settings.Encoding), settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BayatGames.SaveGamePro.Serialization.Formatters.Binary.BinaryObjectWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public BinaryObjectWriterV2(BinaryWriter writer, SaveGameSettings settings)
        {
            this.writer = writer;
            this.settings = settings;
        }

    #endregion

    #region Methods

        /// <summary>
        /// Write the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public virtual void Write<T>(T value)
        {
            Write((object)value);
        }

        /// <summary>
        /// Write the specified value.
        /// </summary>
        /// <param name="value">Value.</param>
        public virtual void Write(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Type type = value.GetType();
            UnityObject unityObject = null;

            // Write GUID reference when the value is an Unity object or type information when there is no GUID for this object
            if (type.IsSubclassOf<UnityObject>())
            {
                unityObject = (UnityObject)value;
                string guid = string.Empty;
                bool hasReference = false;
                if (SaveGameReferenceManager.Current != null)
                {
                    SaveGameReferenceManager referenceManager = SaveGameReferenceManager.Current;
                    guid = referenceManager.Get(unityObject);
                    hasReference = true;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("The current scene doesn't have a Save Game Reference Manager");
                }

                this.writer.Write(hasReference);
                if (hasReference)
                {
                    this.writer.Write(guid);
                }
                else
                {
                    WriteTypeInfo(type);
                }
            }

            // Basic types
            if (type == typeof(string))
            {
                this.writer.Write(Convert.ToString(value));
            }
            else if (type == typeof(decimal))
            {
                this.writer.Write(Convert.ToDecimal(value));
            }
            else if (type == typeof(float))
            {
                this.writer.Write(Convert.ToSingle(value));
            }
            else if (type == typeof(double))
            {
                this.writer.Write(Convert.ToDouble(value));
            }
            else if (type == typeof(bool))
            {
                this.writer.Write(Convert.ToBoolean(value));
            }
            else if (type == typeof(char))
            {
                this.writer.Write(Convert.ToChar(value));
            }
            else if (type == typeof(short))
            {
                this.writer.Write(Convert.ToInt16(value));
            }
            else if (type == typeof(int))
            {
                this.writer.Write(Convert.ToInt32(value));
            }
            else if (type == typeof(long))
            {
                this.writer.Write(Convert.ToInt64(value));
            }
            else if (type == typeof(ushort))
            {
                this.writer.Write(Convert.ToUInt16(value));
            }
            else if (type == typeof(uint))
            {
                this.writer.Write(Convert.ToUInt32(value));
            }
            else if (type == typeof(ulong))
            {
                this.writer.Write(Convert.ToUInt64(value));
            }
            else if (type == typeof(byte))
            {
                this.writer.Write(Convert.ToByte(value));
            }
            else if (type == typeof(sbyte))
            {
                this.writer.Write(Convert.ToSByte(value));
            }
            else if (type.IsEnum)
            {
                this.writer.Write(value.ToString());
            }
            else if (type == typeof(DateTime))
            {
                DateTime dateTime = Convert.ToDateTime(value);
                writer.Write(dateTime.ToString());
            }
            else if (type == typeof(TimeSpan))
            {
                TimeSpan timeSpan = (TimeSpan)value;
                writer.Write(timeSpan.ToString());
            }
            else if (type == typeof(Guid))
            {
                Guid guid = (Guid)value;
                writer.Write(guid.ToString());
            }

            // Array
            else if (type.IsArray)
            {
                Array array = value as Array;
                int[] indices = new int[array.Rank];

                // Array rank
                this.writer.Write(array.Rank);
                for (int i = 0; i < array.Rank; i++)
                {

                    // Array rank length
                    this.writer.Write(array.GetLength(i));
                    indices[i] = array.GetLowerBound(i);
                }
                indices[array.Rank - 1]--;
                bool complete = false;
                while (!complete)
                {
                    indices[array.Rank - 1]++;
                    for (int i = array.Rank - 1; i >= 0; i--)
                    {
                        if (indices[i] > array.GetUpperBound(i))
                        {
                            if (i == 0)
                            {
                                complete = true;
                                break;
                            }
                            for (int j = i; j < array.Rank; j++)
                            {
                                indices[j] = array.GetLowerBound(j);
                            }
                            indices[i - 1]++;
                        }
                    }
                    if (!complete)
                    {
                        Write(array.GetValue(indices));
                    }
                }
            }

            // Sorrugates
            else if (SaveGameTypeManager.HasType(type))
            {
                SaveGameType saveGameType = SaveGameTypeManager.GetType(type);
                saveGameType.Write(value, this);
            }

            // ISavable interface
            else if (value is ISavable)
            {
                ISavable savable = (ISavable)value;
                savable.OnWrite(this);
            }

            // ISerializable interface
            else if (value is ISerializable)
            {
                ISerializable serializable = (ISerializable)value;
                SerializationInfo info = new SerializationInfo(type, new FormatterConverter());
                serializable.GetObjectData(info, new StreamingContext(StreamingContextStates.All));
                var enumerator = info.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    WriteTypeInfo(enumerator.ObjectType);
                    this.writer.Write(enumerator.Name);
                    Write(enumerator.Value);
                }
            }

            // Collections (List<>, SortedList<>, HashSet<>, ...)
            else if (value is ICollection)
            {
                ICollection collection = (ICollection)value;
                IEnumerator enumerator = collection.GetEnumerator();
                this.writer.Write(collection.Count);

                // Dictionaries (Dictionry<>, SortedDictionary<>)
                if (value is IDictionary)
                {
                    IDictionary dictionary = (IDictionary)value;
                    foreach (var dicKey in dictionary.Keys)
                    {
                        var dicValue = dictionary[dicKey];
                        Write(dicKey);
                        Write(dicValue);
                    }
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        Write(enumerator.Current);
                    }
                }
            }

            else if (value is UnityEngine.GameObject)
            {
                GameObject gameObject = (GameObject)value;
                UnityComponent[] components = gameObject.GetComponents<UnityComponent>();

                // Write game object properties
                this.writer.Write(gameObject.name);
                this.writer.Write(gameObject.layer);
                this.writer.Write(gameObject.tag);
                this.writer.Write(gameObject.activeSelf);
                this.writer.Write(gameObject.isStatic);
                Write(gameObject.hideFlags);

                // Write game object components
                this.writer.Write(components.Length);
                for (int i = 0; i < components.Length; i++)
                {
                    UnityComponent component = components[i];
                    Write(component);
                }

                // Write game object childs
                this.writer.Write(gameObject.transform.childCount);
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform childTransform = gameObject.transform.GetChild(i);
                    Write(childTransform.gameObject);
                }
            }

            // Object types - dynamically find and save fields and properties of object
            else
            {

                // Save the object type information, to be used in loading phase (mostly for abstract classes or interfaces)
                if (!type.IsSubclassOf<UnityObject>())
                {
                    WriteTypeInfo(type);
                }
                WriteSavableMembers(value, type);
            }
        }

        public virtual void WriteTypeInfo(Type type)
        {
            this.writer.Write(type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Writes the savable members.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public virtual void WriteSavableMembers(object value, Type type)
        {
            List<FieldInfo> fields = type.GetSavableFields();
            List<PropertyInfo> properties = type.GetSavableProperties();

            this.writer.Write(fields.Count);
            for (int i = 0; i < fields.Count; i++)
            {
                FieldInfo field = fields[i];
                WriteTypeInfo(field.FieldType);
                Write(field.GetValue(value));
            }

            this.writer.Write(properties.Count);
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyInfo property = properties[i];
                WriteTypeInfo(property.PropertyType);
                Write(property.GetValue(value));
            }
        }

        /// <summary>
        /// Writes the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        public virtual void WriteProperty<T>(string identifier, T value)
        {
            WriteProperty(identifier, (object)value);
        }

        /// <summary>
        /// Writes the property.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        public virtual void WriteProperty(string identifier, object value)
        {
            this.writer.Write(identifier);
            long position = this.writer.BaseStream.Position;

            // Preserve skip position
            this.writer.Write(0L);
            Write(value);

            // Write skip position
            long lastPosition = this.writer.BaseStream.Position;
            this.writer.BaseStream.Position = position;
            this.writer.Write(lastPosition);
            this.writer.BaseStream.Position = lastPosition;
        }

        /// <summary>
        /// Releases the resources being used by this class.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.writer != null)
            {
                (this.writer as IDisposable).Dispose();
            }
        }

    #endregion

    }

#endif

}