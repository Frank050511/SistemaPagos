import { Disclosure, DisclosureButton, DisclosurePanel, Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/react'
import { Bars3Icon, BellIcon, XMarkIcon } from '@heroicons/react/24/outline'
import LogoutButton from '../Components/LogoutButton.jsx'
import TablaPlanillas from '../Components/TablaPlanillas.jsx'
import { saveAs } from 'file-saver';

export default function PlanillasAdmin() {
    // Datos de ejemplo - reemplazar con los datos reales de tu API
    const planillas = [
        { id: 1, nombre: 'Planilla_Enero_2023.xlsx', fechaCorte: '2023-01-31', ruta: '/uploads/planillas/planilla1.xlsx' },
        { id: 2, nombre: 'Planilla_Febrero_2023.xlsx', fechaCorte: '2023-02-28', ruta: '/uploads/planillas/planilla2.xlsx' },
        { id: 3, nombre: 'Planilla_Marzo_2023.xlsx', fechaCorte: '2023-03-31', ruta: '/uploads/planillas/planilla3.xlsx' },
        { id: 4, nombre: 'Planilla_Abril_2023.xlsx', fechaCorte: '2023-04-30', ruta: '/uploads/planillas/planilla4.xlsx' },
        { id: 5, nombre: 'Planilla_Mayo_2023.xlsx', fechaCorte: '2023-05-31', ruta: '/uploads/planillas/planilla5.xlsx' },
        { id: 6, nombre: 'Planilla_Junio_2023.xlsx', fechaCorte: '2023-06-30', ruta: '/uploads/planillas/planilla6.xlsx' },
        { id: 7, nombre: 'Planilla_Julio_2023.xlsx', fechaCorte: '2023-07-31', ruta: '/uploads/planillas/planilla7.xlsx' },
    ];

    const handleDescargarPlantilla = async () => {
        try {
            // Obtener el token de autenticación (ajusta según tu implementación)
            const token = localStorage.getItem('token');

            const response = await fetch('api/planillas/plantilla', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                credentials: 'include' // Necesario si usas cookies
            });

            if (!response.ok) {
                throw new Error(`Error ${response.status}: ${response.statusText}`);
            }

            const blob = await response.blob();
            saveAs(blob, 'Plantilla_Boletas.xlsx');

        } catch (error) {
            console.error('Error al descargar la plantilla:', error);
            alert(`Error al descargar la plantilla: ${error.message}`);
        }
    };

    return (
        <>
            <div className="min-h-full">
                <header className="relative bg-gray-800 after:pointer-events-none after:absolute after:inset-x-0 after:inset-y-0 after:border-y after:border-white/10">
                    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8 flex justify-between items-center">
                        <h1 className="text-3xl font-bold tracking-tight text-white">Administrador de planillas</h1>
                        <div className="flex items-center space-x-4">
                            <LogoutButton />
                        </div>
                    </div>
                </header>
                <main>
                    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8">
                        <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6 lg:px-8 flex justify-between items-center">
                            <button
                                onClick={handleDescargarPlantilla}
                                className="bg-indigo-500 hover:bg-indigo-600 text-white px-4 py-2 rounded transition-colors"
                            >
                                Descargar Plantilla
                            </button>
                            <button
                                onClick=""
                                className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded transition-colors"
                            >
                                Agregar planilla
                            </button>
                        </div>

                        {/* Componente TablaPlanillas con barra de búsqueda y paginación */}
                        <TablaPlanillas planillas={planillas} />
                    </div>
                </main>
            </div>
        </>
    )
}