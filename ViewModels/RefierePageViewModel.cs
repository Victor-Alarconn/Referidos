﻿using MySqlConnector;
using Plugin.DeviceInfo;
using Referidos.Data;
using Referidos.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Referidos.ViewModels
{
    public class RefierePageViewModel : INotifyPropertyChanged
    {
        public ICommand EnviarRefeCommand { get; private set; }

        public RefierePageViewModel()
        {
            EnviarRefeCommand = new Command(EnviarRefe);
        }

        private List<string> _tiposReferencia = new List<string>
        {
            "Sector Comercial",
            "Servicios",
            "Restaurante/Bar",
            "Hotel",
            "Taller",
            "Créditos",
            "Sindicatos",
            "Divisas",
            "Parqueaderos",
            "Transporte",
            "Control de obra",
            "Otro"
        };

        public List<string> TiposReferencia
        {
            get => _tiposReferencia;
        }


        // Propiedades para enlazar los campos del formulario con el ViewModel
        private string _nombreCompletoR;
        public string NombreCompletoRefe
        {
            get => _nombreCompletoR;
            set
            {
                _nombreCompletoR = value;
                OnPropertyChanged(nameof(NombreCompletoRefe));
            }
        }

        private string _correoR;
        public string CorreoRefe
        {
            get => _correoR;
            set
            {
                _correoR = value;
                OnPropertyChanged(nameof(CorreoRefe));
            }
        }

        private int _telefonoR;
        public int TelefonoRefe
        {
            get => _telefonoR;
            set
            {
                _telefonoR = value;
                OnPropertyChanged(nameof(TelefonoRefe));
            }
        }

        private string _tipoR;
        public string TipoRefe
        {
            get => _tipoR;
            set
            {
                _tipoR = value;
                OnPropertyChanged(nameof(TipoRefe));
            }
        }

        private string _empresaR;
        public string EmpresaRefe
        {
            get => _empresaR;
            set
            {
                _empresaR = value;
                OnPropertyChanged(nameof(EmpresaRefe));
            }
        }

        private string _notasR;
        public string NotasRefe
        {
            get => _notasR;
            set
            {
                _notasR = value;
                OnPropertyChanged(nameof(NotasRefe));
            }
        }

        private async void EnviarRefe()
        {
            
            string id = CrossDeviceInfo.Current.Id;
            string NombreUsuario = Preferences.Get("NombreUsuarioCache", string.Empty);

            // Validar que los campos requeridos no estén vacíos
            if (string.IsNullOrEmpty(NombreCompletoRefe) || TelefonoRefe == 0)
            {
                // Mostrar el mensaje de error utilizando DisplayAlert
                await Application.Current.MainPage.DisplayAlert("Error", "Faltan campos por rellenar.", "OK");
                return; // Salir del método si hay campos requeridos vacíos
            }

            try
            {
                using MySqlConnection connection = DataConexion.ObtenerConexion();
                connection.Open();

                string query = "INSERT INTO bs_main (bs_Nombre, bs_Email, bs_Telefono1, bs_Empresa, bs_Tipo, bs_Fecha, bs_estado, bs_Equipo, bs_Notas, bs_Refiere) " +
               "VALUES (@NombreCompleto, @Correo, @Telefono, @Empresa, @Tipo, @FechaIngreso, @Estado, @Mac, @Notas, @Refiere)";

                using MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@NombreCompleto", NombreCompletoRefe);
                cmd.Parameters.AddWithValue("@Correo", CorreoRefe);
                cmd.Parameters.AddWithValue("@Telefono", TelefonoRefe);
                cmd.Parameters.AddWithValue("@Empresa", EmpresaRefe);
                cmd.Parameters.AddWithValue("@Tipo", TipoRefe);
                // Agregar la fecha actual al parámetro @FechaIngreso
                cmd.Parameters.AddWithValue("@FechaIngreso", DateTime.Now);
                // Establecer el valor de bs_estado como 0
                cmd.Parameters.AddWithValue("@Estado", 0);
                cmd.Parameters.AddWithValue("@Mac", id);
                cmd.Parameters.AddWithValue("@Notas", NotasRefe);
                cmd.Parameters.AddWithValue("@Refiere", NombreUsuario);
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
            await Application.Current.MainPage.DisplayAlert("Registro Exitoso", "El cliente fue guardado con éxito.", "OK");

            // Redireccionar a la página PrincipalPage
            await Application.Current.MainPage.Navigation.PushAsync(new PrincipalPage());
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
