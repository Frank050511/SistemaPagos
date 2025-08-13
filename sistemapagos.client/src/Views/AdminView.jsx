import { useState } from 'react';
import LogoutButton from '../Components/LogoutButton.jsx';
import TablaPlanillas from '../Components/TablaPlanillas.jsx';
import { saveAs } from 'file-saver';
import CargarPlanillaModal from '../Components/CargarPlanillaModal.jsx';

export default function PlanillasAdmin() {
    const [isDownloading, setIsDownloading] = useState(false);
    const [error, setError] = useState(null);
    const [showUploadModal, setShowUploadModal] = useState(false);
    const [reloadTrigger, setReloadTrigger] = useState(0);

    const handleDescargarPlantilla = async () => {
        setIsDownloading(true);
        setError(null);

        try {
            const response = await fetch('https://localhost:7258/api/planillas/plantilla', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Accept': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
                },
                credentials: 'include'
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText || 'Error al descargar la plantilla');
            }

            const blob = await response.blob();
            if (blob.size === 0) {
                throw new Error('El archivo recibido está vacío');
            }

            saveAs(blob, 'Plantilla_Boletas.xlsx');
        } catch (err) {
            console.error('Error al descargar:', err);
            setError(err.message);
        } finally {
            setIsDownloading(false);
        }
    };

    const handleUploadSuccess = async (formData) => {
        try {
            const response = await fetch('https://localhost:7258/api/planillas/cargar', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: formData,
                credentials: 'include'
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Error al cargar la planilla');
            }

            setReloadTrigger(prev => prev + 1);
            setShowUploadModal(false);
            setError(null);
        } catch (err) {
            setError(err.message);
            throw err;
        }
    };

    return (
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
                    <div className="flex justify-between items-center mb-6">
                        <button
                            onClick={handleDescargarPlantilla}
                            disabled={isDownloading}
                            className={`bg-indigo-500 text-white px-4 py-2 rounded transition-colors ${isDownloading ? 'opacity-50 cursor-not-allowed' : 'hover:bg-indigo-600'
                                }`}
                        >
                            {isDownloading ? 'Descargando...' : 'Descargar Plantilla'}
                        </button>

                        <button
                            onClick={() => setShowUploadModal(true)}
                            className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded transition-colors"
                        >
                            Agregar planilla
                        </button>
                    </div>

                    {error && (
                        <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-4" role="alert">
                            <p>Error: {error}</p>
                        </div>
                    )}

                    <TablaPlanillas reloadTrigger={reloadTrigger} />
                </div>
            </main>

            <CargarPlanillaModal
                isOpen={showUploadModal}
                onClose={() => {
                    setShowUploadModal(false);
                    setError(null);
                }}
                onUploadSuccess={handleUploadSuccess}
            />
        </div>
    );
}