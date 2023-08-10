﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Referidos.Data;
using Xamarin.Essentials;
using Plugin.DeviceInfo;
using Preferences = Microsoft.Maui.Storage.Preferences;

namespace Referidos.ViewModels
{
    public class RegistroUsuarioViewModel : INotifyPropertyChanged
    {
        public ICommand EnviarCommand { get; private set; }

        public RegistroUsuarioViewModel()
        {
            EnviarCommand = new Command(Enviar);
        }

        // Propiedades para enlazar los campos del formulario con el ViewModel
        private string _nombreCompleto;
        public string NombreCompleto
        {
            get => _nombreCompleto;
            set
            {
                _nombreCompleto = value;
                OnPropertyChanged(nameof(NombreCompleto));
            }
        }

        private string _cedula;
        public string Cedula
        {
            get => _cedula;
            set
            {
                _cedula = value;
                OnPropertyChanged(nameof(Cedula));
            }
        }

        private string _correo;
        public string Correo
        {
            get => _correo;
            set
            {
                _correo = value;
                OnPropertyChanged(nameof(Correo));
            }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set
            {
                _telefono = value;
                OnPropertyChanged(nameof(Telefono));
            }
        }

        private string _ciudad;
        public string Ciudad
        {
            get => _ciudad;
            set
            {
                _ciudad = value;
                OnPropertyChanged(nameof(Ciudad));
            }
        }

        private string _asesor;
        public string Asesor
        {
            get => _asesor;
            set
            {
                _asesor = value;
                OnPropertyChanged(nameof(Asesor));
            }
        }

        private async void Enviar()
        {

            string id = CrossDeviceInfo.Current.Id;

            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrEmpty(NombreCompleto) || string.IsNullOrEmpty(Correo) || !int.TryParse(Cedula, out int cedula) || cedula == 0 || !int.TryParse(Telefono, out int numeroTelefono) || numeroTelefono == 0 || string.IsNullOrEmpty(Ciudad))
            {
                // Mostrar el mensaje de error utilizando DisplayAlert
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan campos por rellenar.", "OK");
                return; // Salir del método si hay campos requeridos vacíos
            }

            // Guardar el nombre en la caché
            Preferences.Set("NombreUsuarioCache", NombreCompleto);

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "INSERT INTO bs_refe (bs_nombre, bs_cedula, bs_correo, bs_telefono, bs_ciudad, bs_vend, bs_fingreso, bs_estado, bs_mac) " +
               "VALUES (@NombreCompleto, @Cedula, @Correo, @Telefono, @Ciudad, @Asesor, @FechaIngreso, @Estado)";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreCompleto", NombreCompleto);
                cmd.Parameters.AddWithValue("@Cedula", Cedula);
                cmd.Parameters.AddWithValue("@Correo", Correo);
                cmd.Parameters.AddWithValue("@Telefono", Telefono);
                cmd.Parameters.AddWithValue("@Ciudad", Ciudad);
                cmd.Parameters.AddWithValue("@Asesor", Asesor);
                // Agregar la fecha actual al parámetro @FechaIngreso
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                // Establecer el valor de bs_estado como 0
                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Mac", id);
                cmd.ExecuteNonQuery();

                // Mostrar la alerta de éxito si el registro fue exitoso
                await MostrarAlertaExito();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        private async Task MostrarAlertaExito()
        {
            // Mostrar el mensaje de éxito utilizando DisplayAlert
            await Application.Current.MainPage.DisplayAlert("Registro Exitoso", "Pronto le enviaremos una clave de activación.", "OK");
            LimpiarDatos();
            // Redireccionar a la página de inicio (HomePage)
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }
        private void LimpiarDatos()
        {
            NombreCompleto = string.Empty;
            Telefono = string.Empty;
            Cedula = string.Empty;
            Correo = string.Empty;
            Asesor = string.Empty;
            Ciudad = string.Empty;
            // Y cualquier otro campo que desees restablecer
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
