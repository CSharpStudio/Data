using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    [Serializable]
    public class VarFieldData
    {
        IVarField[] _fieldData;

        VarFieldData()
        { /* exists to support MobileFormatter */ }

        internal VarFieldData(VarPropertyContainer container)
        {
            _fieldData = new IVarField[container.ReadWriteProperties.Count()];
        }

        IVarField GetOrCreateField(IVarProperty property)
        {
            try
            {
                var field = _fieldData[property.TypeCompiledIndex];
                if (field == null)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType is VarType)
                        propertyType = ((VarType)propertyType).Type;
                    var type = typeof(VarField<>).MakeGenericType(propertyType);
                    field = Activator.CreateInstance(type, property.Name) as IVarField;
                    _fieldData[property.TypeCompiledIndex] = field;
                }
                return field;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException("Property [{0}] Not Registered".FormatArgs(property.Name), ex);
            }
        }

        /// <summary>
        /// Get field
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        public IVarField GetField(IVarProperty property)
        {
            try
            {
                return _fieldData[property.TypeCompiledIndex];
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException("Property [{0}] Not Registered".FormatArgs(property.Name), ex);
            }
        }

        /// <summary>
        /// Sets the value for a specific field.
        /// </summary>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        /// <param name="value">
        /// Value to store for field.
        /// </param>
        public void SetField(IVarProperty property, object value)
        {
            value = value.ConvertTo(property.PropertyType);
            var field = GetOrCreateField(property);
            field.Value = value;
        }

        /// <summary>
        /// Sets the value for a specific field.
        /// </summary>
        /// <typeparam name="P">
        /// Type of field value.
        /// </typeparam>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        /// <param name="value">
        /// Value to store for field.
        /// </param>
        public void SetField<P>(IVarProperty property, P value)
        {
            var field = GetOrCreateField(property);
            var fd = field as IVarField<P>;
            if (fd != null)
                fd.Value = value;
            else
                field.Value = value;
        }

        /// <summary>
        /// Sets the value for a specific field without
        /// marking the field as dirty.
        /// </summary>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        /// <param name="value">
        /// Value to store for field.
        /// </param>
        public IVarField LoadField(IVarProperty property, object value)
        {
            var field = GetOrCreateField(property);
            field.Value = value.ConvertTo(property.PropertyType);
            field.MarkClean();
            return field;
        }

        /// <summary>
        /// Sets the value for a specific field without
        /// marking the field as dirty.
        /// </summary>
        /// <typeparam name="P">
        /// Type of field value.
        /// </typeparam>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        /// <param name="value">
        /// Value to store for field.
        /// </param>
        internal IVarField LoadField<P>(IVarProperty property, P value)
        {
            var field = GetOrCreateField(property);
            var fd = field as IVarField<P>;
            if (fd != null)
                fd.Value = value;
            else
                field.Value = value;
            field.MarkClean();
            return field;
        }

        /// <summary>
        /// Removes the value for a specific field.
        /// The <see cref="IManagedField" /> object is
        /// not removed, only the contained field value.
        /// </summary>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        public void RemoveField(IVarProperty property)
        {
            try
            {
                var field = _fieldData[property.TypeCompiledIndex];
                if (field != null)
                    field.Value = null;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException("Property [{0}] Not Registered".FormatArgs(property.Name), ex);
            }
        }

        /// <summary>
        /// Returns a value indicating whether an
        /// <see cref="IManagedField" /> entry exists
        /// for the specified property.
        /// </summary>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        public bool FieldExists(IVarProperty property)
        {
            try
            {
                return _fieldData[property.TypeCompiledIndex] != null;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException("Property [{0}] Not Registered".FormatArgs(property.Name), ex);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified field
        /// has been changed.
        /// </summary>
        /// <param name="property">
        /// The property corresponding to the field.
        /// </param>
        /// <returns>True if the field has been changed.</returns>
        public bool IsFieldDirty(IVarProperty property)
        {
            try
            {
                bool result = false;
                var field = _fieldData[property.TypeCompiledIndex];
                if (field != null)
                    result = field.IsDirty;
                return result;

            }
            catch (IndexOutOfRangeException ex)
            {
                throw new InvalidOperationException("Property [{0}] Not Registered".FormatArgs(property.Name), ex);
            }
        }

        /// <summary>
        /// Returns a value indicating whether any
        /// fields are dirty.
        /// </summary>
        public bool IsDirty()
        {
            foreach (var item in _fieldData)
                if (item != null && item.IsDirty)
                    return true;
            return false;
        }

        /// <summary>
        /// Marks all fields as clean
        /// (not dirty).
        /// </summary>
        public void MarkClean()
        {
            foreach (var item in _fieldData)
                if (item != null && item.IsDirty)
                    item.MarkClean();
        }
    }
}
