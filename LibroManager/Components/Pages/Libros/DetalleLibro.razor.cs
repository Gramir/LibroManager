using LibroManager.DTOs;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LibroManager.Components.Pages.Libros;

public partial class DetalleLibro
{
    [Inject]
    private ILibroService LibroService { get; set; } = default!;

    [Inject]
    private IPrestamoService PrestamoService { get; set; } = default!;

    [Inject]
    private IUbicacionService UbicacionService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    private LibroDTO? _libro;
    private IEnumerable<LibroDTO> _ejemplares = [];
    private string _errorMessage = string.Empty;
    private bool _mostrarConfirmacion;
    private LibroDTO? _ejemplarAEliminar;
    private bool _mostrarFormularioEjemplar;
    private bool _mostrarFormularioEditarEjemplar;
    private LibroDTO? _ejemplarEditar;
    private string _nuevaUbicacion = string.Empty;
    private bool _showDropdown;
    private IEnumerable<UbicacionDTO>? _ubicaciones;
    private IEnumerable<UbicacionDTO>? _ubicacionesFiltradas;
    private string _errorMessageUbicacion = string.Empty;
    private bool _tienePrestamosHistoricos;
    private string? _proximoSerial;

    [Parameter]
    public int LibroId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _libro = await LibroService.GetLibroByIdAsync(LibroId);
        if (_libro == null)
        {
            NavigationManager.NavigateTo("/libros");
            return;
        }

        _ejemplares = await LibroService.GetEjemplaresPorIsbnAsync(_libro.ISBN);
        await LoadUbicaciones();
        _ubicacionesFiltradas = _ubicaciones;
    }

    private async Task LoadUbicaciones()
    {
        if (_ejemplarEditar != null)
        {
            _ubicaciones = await UbicacionService.GetAvailableUbicacionesWithCurrentAsync(_ejemplarEditar.LibroId);
        }
        else
        {
            _ubicaciones = await UbicacionService.GetAvailableUbicacionesAsync();
        }
    }

    private void Volver()
    {
        NavigationManager.NavigateTo("/libros");
    }

    private void EditarLibro()
    {
        NavigationManager.NavigateTo($"/libros/editar/{LibroId}");
    }

    private async Task MostrarConfirmacionEjemplar(LibroDTO ejemplar)
    {
        if (ejemplar.EstaPrestado)
        {
            _errorMessage = "No se puede eliminar un ejemplar que está prestado.";
            return;
        }

        _tienePrestamosHistoricos = await PrestamoService.TienePrestamosHistoricosAsync(ejemplar.LibroId);
        _ejemplarAEliminar = ejemplar;
        _mostrarConfirmacion = true;
    }

    private void MostrarConfirmacion()
    {
        if (_ejemplares.Any())
        {
            _errorMessage = "No se puede eliminar el libro mientras tenga ejemplares. Elimine los ejemplares primero.";
            return;
        }
        _ejemplarAEliminar = null;
        _mostrarConfirmacion = true;
    }

    private void CancelarEliminacion()
    {
        _mostrarConfirmacion = false;
        _ejemplarAEliminar = null;
        _tienePrestamosHistoricos = false;
    }

    private async Task EliminarLibro()
    {
        _mostrarConfirmacion = false;

        try
        {
            bool result;
            if (_ejemplarAEliminar != null)
            {
                if (_tienePrestamosHistoricos)
                {
                    // Primero eliminar los préstamos históricos
                    await PrestamoService.DeletePrestamosByLibroIdAsync(_ejemplarAEliminar.LibroId);
                }

                result = await LibroService.DeleteLibroAsync(_ejemplarAEliminar.LibroId);
                if (result)
                {
                    // Actualizar la lista de ejemplares
                    _ejemplares = await LibroService.GetEjemplaresPorIsbnAsync(_libro!.ISBN);
                    if (!_ejemplares.Any())
                    {
                        // Si era el último ejemplar, volver a la lista
                        NavigationManager.NavigateTo("/libros");
                    }
                }
                else
                {
                    _errorMessage = "No se pudo eliminar el ejemplar. Puede que esté prestado o ya no exista.";
                }
            }
            else
            {
                result = await LibroService.DeleteLibroAsync(LibroId);
                if (result)
                {
                    NavigationManager.NavigateTo("/libros");
                }
                else
                {
                    _errorMessage = "No se pudo eliminar el libro. Puede que ya no exista.";
                }
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error al eliminar: {ex.Message}";
        }
        finally
        {
            _ejemplarAEliminar = null;
            _tienePrestamosHistoricos = false;
        }
    }

    private bool TieneEjemplaresPrestados()
    {
        return _ejemplares.Any(e => e.EstaPrestado);
    }

    private async Task MostrarFormularioEjemplar()
    {
        _errorMessageUbicacion = string.Empty;
        _nuevaUbicacion = string.Empty;
        _mostrarFormularioEjemplar = true;
        if (_libro != null)
        {
            _proximoSerial = await LibroService.GetNextAvailableSerial(_libro.ISBN);
        }
        await LoadUbicaciones();
    }

    private void CerrarFormularioEjemplar()
    {
        _mostrarFormularioEjemplar = false;
        _nuevaUbicacion = string.Empty;
    }

    private void MostrarFormularioEditarEjemplar(LibroDTO ejemplar)
    {
        _ejemplarEditar = ejemplar;
        _mostrarFormularioEditarEjemplar = true;
        _nuevaUbicacion = ejemplar.UbicacionFormateada;
        _showDropdown = false;
    }

    private void CerrarFormularioEditarEjemplar()
    {
        _mostrarFormularioEditarEjemplar = false;
        _nuevaUbicacion = string.Empty;
    }

    private void OnUbicacionInput(ChangeEventArgs e)
    {
        _nuevaUbicacion = e.Value?.ToString() ?? string.Empty;
        FilterUbicaciones(e);
    }

    private void FilterUbicaciones(ChangeEventArgs e)
    {
        var searchText = e.Value?.ToString()?.ToLower() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            _ubicacionesFiltradas = _ubicaciones;
        }
        else
        {
            _ubicacionesFiltradas = _ubicaciones?
                .Where(u => u.UbicacionFormateada.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }
        _showDropdown = true;

        // Validar si el texto actual no coincide con ninguna ubicación disponible
        if (_ubicaciones != null && !string.IsNullOrWhiteSpace(searchText) &&
            !_ubicaciones.Any(u => u.UbicacionFormateada.Equals(_nuevaUbicacion, StringComparison.OrdinalIgnoreCase)))
        {
            _errorMessageUbicacion = "Por favor, seleccione una ubicación válida del listado desplegable.";
        }
        else
        {
            _errorMessageUbicacion = string.Empty;
        }
    }

    private void ShowDropdown()
    {
        _showDropdown = true;
    }

    private void SelectUbicacion(string ubicacion)
    {
        _nuevaUbicacion = ubicacion;
        _showDropdown = false;
        _errorMessageUbicacion = string.Empty;
    }

    private async Task OnBlur()
    {
        await Task.Delay(150);
        _showDropdown = false;
        StateHasChanged();
    }

    private async Task CrearEjemplar()
    {
        if (string.IsNullOrWhiteSpace(_nuevaUbicacion) || _libro == null)
            return;

        // Verificar si la ubicación seleccionada está en la lista de ubicaciones disponibles
        if (_ubicaciones == null || !_ubicaciones.Any(u => u.UbicacionFormateada == _nuevaUbicacion))
        {
            _errorMessageUbicacion = "Debe seleccionar una ubicación válida del listado desplegable.";
            return;
        }

        try
        {
            var result = await LibroService.CreateEjemplarAsync(_libro.ISBN, _nuevaUbicacion);
            if (result)
            {
                _ejemplares = await LibroService.GetEjemplaresPorIsbnAsync(_libro.ISBN);
                await LoadUbicaciones(); // Recargar ubicaciones disponibles
                CerrarFormularioEjemplar();
            }
            else
            {
                _errorMessage = "No se pudo crear el ejemplar. Por favor, intente nuevamente.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error al crear el ejemplar: {ex.Message}";
        }
    }

    private async Task GuardarEjemplar()
    {
        if (string.IsNullOrWhiteSpace(_nuevaUbicacion) || _libro == null || _ejemplarEditar == null)
            return;

        // Verificar si la ubicación seleccionada está en la lista de ubicaciones disponibles
        // o si es la ubicación actual del ejemplar
        if (_ubicaciones == null ||
            (!_ubicaciones.Any(u => u.UbicacionFormateada == _nuevaUbicacion) &&
             _nuevaUbicacion != _ejemplarEditar.UbicacionFormateada))
        {
            _errorMessageUbicacion = "Debe seleccionar una ubicación válida del listado desplegable.";
            return;
        }

        try
        {
            var result = await LibroService.UpdateEjemplarAsync(_ejemplarEditar.LibroId, _nuevaUbicacion);
            if (result)
            {
                _ejemplares = await LibroService.GetEjemplaresPorIsbnAsync(_libro.ISBN);
                await LoadUbicaciones();
                CerrarFormularioEditarEjemplar();
            }
            else
            {
                _errorMessage = "No se pudo guardar el ejemplar. Por favor, intente nuevamente.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error al guardar el ejemplar: {ex.Message}";
        }
    }

    private void PrestarLibro(int libroId)
    {
        NavigationManager.NavigateTo($"/prestamos/crear/{libroId}");
    }

    private async Task VerPrestamo(int libroId)
    {
        var prestamos = await PrestamoService.GetPrestamosByLibroAsync(libroId);
        var prestamo = prestamos
            .OrderByDescending(p => p.FechaVencimiento)
            .FirstOrDefault();

        if (prestamo != null)
        {
            NavigationManager.NavigateTo($"/prestamos/{prestamo.PrestamoId}");
        }
    }

    private static string GetEstadoBadgeClass(LibroDTO ejemplar)
    {
        if (ejemplar.EstaPerdido)
            return "bg-danger";
        if (ejemplar.EstaPrestado)
            return "bg-warning";
        return "bg-success";
    }

    private static string GetEstadoText(LibroDTO ejemplar)
    {
        if (ejemplar.EstaPerdido)
            return "Perdido";
        if (ejemplar.EstaPrestado)
            return "Prestado";
        return "Disponible";
    }
}