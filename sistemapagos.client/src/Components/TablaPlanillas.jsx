import { useState, useEffect, useCallback } from 'react';
import { MagnifyingGlassIcon } from '@heroicons/react/24/solid';
import { saveAs } from 'file-saver';

const TablaPlanillas = ({ reloadTrigger }) => {
    const [planillas, setPlanillas] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const itemsPerPage = 5; // Número de planillas que se muestran en la tabla por página

    const fetchPlanillas = useCallback(async () => {
        try {
            const response = await fetch('https://localhost:7258/api/planillas', {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                throw new Error('Error al obtener las planillas');
            }

            const data = await response.json();
            setPlanillas(Array.isArray(data) ? data : []);
            console.log("Respuesta del servidor:", data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchPlanillas();
    }, [fetchPlanillas, reloadTrigger]); // Vuelve a cargar las planillas cuando cambia reloadTrigger

    // Función para descargar una planilla
    const handleDescargarPlanilla = async (rutaArchivo, nombrePlanilla) => {
        try {
            const response = await fetch(`https://localhost:7258/api/planillas/descargar?ruta=${encodeURIComponent(rutaArchivo)}`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                throw new Error('Error al descargar la planilla');
            }

            const blob = await response.blob();
            saveAs(blob, nombrePlanilla); // Usamos el nombre original del archivo para la descarga
        } catch (err) {
            setError(err.message);
        }
    };

    // Función para eliminar una planilla a través de su ID (no la elimina del servidor, solo de la vista).
    const handleEliminarPlanilla = async (idPlanilla) => {
        try {
            const response = await fetch(`https://localhost:7258/api/planillas/${idPlanilla}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

            if (!response.ok) {
                throw new Error('Error al eliminar la planilla');
            }

            // Actualizar la lista de planillas
            setPlanillas(prevPlanillas => prevPlanillas.filter(p => p.idPlanilla !== idPlanilla));
        } catch (err) {
            setError(err.message);
        }
    };

    // Filtrar planillas al utilizar la barra de búsqueda
    const filteredPlanillas = planillas.filter(planilla => {
        if (!planilla || typeof planilla !== 'object') return false;

        const nombre = planilla.nombrePlanilla || '';
        const fechaCorte = planilla.fechaCorte || '';

        return nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
            fechaCorte.includes(searchTerm);
    });

    // Calcular el total de páginas
    const totalPages = Math.ceil(filteredPlanillas.length / itemsPerPage);

    // Obtener planillas para la página actual
    const currentPlanillas = filteredPlanillas.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    if (loading) return <div className="p-4 text-center">Cargando planillas...</div>;
    if (error) return <div className="p-4 text-red-500 text-center">Error: {error}</div>;

    return (
        <div className="space-y-4">
            {/* Barra de búsqueda */}
            <div className="relative max-w-md">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <MagnifyingGlassIcon className="h-5 w-5 text-gray-400" />
                </div>
                <input
                    type="text"
                    value={searchTerm}
                    onChange={(e) => {
                        setSearchTerm(e.target.value); 
                        setCurrentPage(1);
                    }}
                    className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                    placeholder="Buscar planilla..."
                />
            </div>

            {/* Tabla */}
            <div className="overflow-hidden shadow ring-1 ring-black ring-opacity-5 rounded-lg">
                <table className="min-w-full divide-y divide-gray-300">
                    <thead className="bg-gray-50">
                        <tr>
                            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Nombre del archivo
                            </th>
                            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Fecha de corte
                            </th>
                            <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                Acciones
                            </th>
                        </tr>
                    </thead>
                    {/* Cuerpo de la tabla donde se agregan las planillas activas */}
                    <tbody className="bg-white divide-y divide-gray-200">
                        {currentPlanillas.length > 0 ? (
                            currentPlanillas.map((planilla) => (
                                <tr key={planilla.idPlanilla}>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                        {planilla.nombrePlanilla || 'Sin nombre'}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        {planilla.fechaCorte ? new Date(planilla.fechaCorte).toLocaleDateString() : 'Sin fecha'}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        <div className="flex space-x-2">
                                            <button
                                                onClick={() => handleDescargarPlanilla(planilla.rutaArchivo, planilla.nombrePlanilla)}
                                                className="text-indigo-600 hover:text-indigo-900"
                                            >
                                                Descargar
                                            </button>
                                            <button
                                                onClick={() => handleEliminarPlanilla(planilla.idPlanilla)}
                                                className="text-red-600 hover:text-red-900"
                                            >
                                                Eliminar
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="3" className="px-6 py-4 text-center text-sm text-gray-500">
                                    No se encontraron planillas
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {/* Paginación */}
            {totalPages > 1 && (
                <div className="flex items-center justify-between border-t border-gray-200 px-4 py-3 sm:px-6">
                    <div className="flex flex-1 justify-between sm:hidden">
                        <button
                            onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                            disabled={currentPage === 1}
                            className="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
                        >
                            Anterior
                        </button>
                        <button
                            onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                            disabled={currentPage === totalPages}
                            className="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
                        >
                            Siguiente
                        </button>
                    </div>
                    <div className="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
                        <div>
                            {/* Muestra el rango de resultados actuales y el total de planillas */  }
                            <p className="text-sm text-gray-700">
                                Mostrando <span className="font-medium">{(currentPage - 1) * itemsPerPage + 1}</span> a{' '}
                                <span className="font-medium">{Math.min(currentPage * itemsPerPage, filteredPlanillas.length)}</span> de{' '}
                                <span className="font-medium">{filteredPlanillas.length}</span> resultados 
                            </p>
                        </div>
                        <div>
                            <nav className="isolate inline-flex -space-x-px rounded-md shadow-sm" aria-label="Pagination">
                                <button
                                    onClick={() => setCurrentPage(1)}
                                    disabled={currentPage === 1}
                                    className="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                                >
                                    <span className="sr-only">Primera</span>
                                    &laquo;
                                </button>
                                <button
                                    onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                                    disabled={currentPage === 1}
                                    className="relative inline-flex items-center px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                                >
                                    <span className="sr-only">Anterior</span>
                                    &lsaquo;
                                </button>

                                {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                                    let pageNum;
                                    if (totalPages <= 5) {
                                        pageNum = i + 1;
                                    } else if (currentPage <= 3) {
                                        pageNum = i + 1;
                                    } else if (currentPage >= totalPages - 2) {
                                        pageNum = totalPages - 4 + i;
                                    } else {
                                        pageNum = currentPage - 2 + i;
                                    }

                                    return (
                                        <button
                                            key={pageNum}
                                            onClick={() => setCurrentPage(pageNum)}
                                            className={`relative inline-flex items-center px-4 py-2 text-sm font-semibold ${currentPage === pageNum ? 'bg-indigo-600 text-white' : 'text-gray-200 ring-1 ring-inset ring-gray-300 hover:bg-gray-50'}`}
                                        >
                                            {pageNum}
                                        </button>
                                    );
                                })}

                                <button
                                    onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                                    disabled={currentPage === totalPages}
                                    className="relative inline-flex items-center px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                                >
                                    <span className="sr-only">Siguiente</span>
                                    &rsaquo;
                                </button>
                                <button
                                    onClick={() => setCurrentPage(totalPages)}
                                    disabled={currentPage === totalPages}
                                    className="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0"
                                >
                                    <span className="sr-only">Última</span>
                                    &raquo;
                                </button>
                            </nav>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default TablaPlanillas;