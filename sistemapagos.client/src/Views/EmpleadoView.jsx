// ... resto de imports
import { useState, useEffect, useCallback } from 'react';
import { Disclosure, DisclosureButton, DisclosurePanel, Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/react'
import { Bars3Icon, BellIcon, XMarkIcon } from '@heroicons/react/24/outline'
import "../index.css"
import LogoutButton from '../Components/LogoutButton.jsx'
import BoletaCard from '../Components/BoletaCard.jsx'
export default function Empleado() {
    const [boletas, setBoletas] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchParams, setSearchParams] = useState({
        anio: new Date().getFullYear(),
        mes: new Date().getMonth() + 1
    });

    const fetchBoletas = useCallback(async () => {
        try {
            setLoading(true);
            const queryParams = new URLSearchParams({
                anio: searchParams.anio,
                mes: searchParams.mes
            }).toString();

            const response = await fetch(`https://localhost:7258/api/Boletas?${queryParams}`, {
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Error al obtener las boletas');
            }

            const data = await response.json();
            setBoletas(data);
        } catch (error) {
            console.error("Error fetching boletas:", error);
        } finally {
            setLoading(false);
        }
    }, [searchParams.anio, searchParams.mes]);

    useEffect(() => {
        fetchBoletas();
    }, [fetchBoletas]);

    const handleSearch = (e) => {
        e.preventDefault();
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('es-ES', { day: 'numeric', month: 'long', year: 'numeric' });
    };

    // Agrupar por Corte
    const groupByDate = (boletas) => {
        return boletas.reduce((acc, boleta) => {
            const dateKey = boleta.corte; // Ojo con minúscula si backend devuelve camelCase
            if (!acc[dateKey]) {
                acc[dateKey] = [];
            }
            acc[dateKey].push(boleta);
            return acc;
        }, {});
    };

    const groupedBoletas = groupByDate(boletas);

    return (
        <div className="min-h-full">
            <header className="relative bg-gray-800">
                <div className="mx-auto max-w-7xl px-4 py-6 flex justify-between items-center">
                    <h1 className="text-3xl font-bold tracking-tight text-white">Boleta de pagos</h1>
                    <LogoutButton />
                </div>
            </header>

            <main className="mx-auto max-w-7xl px-4 py-6">
                {/* Formulario búsqueda */}
                <div className="bg-gray-700 p-4 rounded-lg mb-6">
                    <form onSubmit={handleSearch} className="flex flex-wrap gap-4">
                        <div>
                            <label htmlFor="anio" className="block text-sm font-medium text-white mb-1">
                                Año
                            </label>
                            <input
                                type="number"
                                id="anio"
                                value={searchParams.anio}
                                onChange={(e) => setSearchParams(prev => ({ ...prev, anio: e.target.value }))}
                                className="p-2 rounded text-gray-400 border border-gray-300"
                                min="2000"
                                max="2100"
                            />
                        </div>
                        <div>
                            <label htmlFor="mes" className="block text-sm font-medium text-white mb-1">
                                Mes
                            </label>
                            <select
                                id="mes"
                                value={searchParams.mes}
                                onChange={(e) => setSearchParams(prev => ({ ...prev, mes: e.target.value }))}
                                className="p-2 rounded text-gray-400 border border-gray-300"
                            >
                                {Array.from({ length: 12 }, (_, i) => i + 1).map(month => (
                                    <option key={month} value={month}>
                                        {new Date(2000, month - 1, 1).toLocaleDateString('es-ES', { month: 'long' })}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </form>
                </div>

                {/* Resultados */}
                {loading ? (
                    <div className="flex justify-center items-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
                    </div>
                ) : boletas.length === 0 ? (
                    <div className="text-center text-white py-8">
                        No se encontraron boletas para el periodo seleccionado
                    </div>
                ) : (
                    Object.entries(groupedBoletas).map(([date, boletasForDate]) =>
                        boletasForDate.map((boleta, index) => (
                            <BoletaCard
                                key={`${date}-${index}`}
                                title={`Boleta - ${formatDate(date)}`}
                            >
                                <div className="overflow-x-auto">
                                    <table className="min-w-max bg-white">
                                        <thead className="bg-gray-100">
                                            <tr>
                                                <th className="px-4 py-2 border">Código Empleado</th>
                                                <th className="px-4 py-2 border">Nombre</th>
                                                <th className="px-4 py-2 border">Salario Bruto</th>
                                                <th className="px-4 py-2 border">ISSS</th>
                                                <th className="px-4 py-2 border">AFP</th>
                                                <th className="px-4 py-2 border">Renta</th>
                                                <th className="px-4 py-2 border">Salario Neto</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td className="px-4 py-2 border">{boleta.codigoEmpleado}</td>
                                                <td className="px-4 py-2 border">{boleta.nombreEmpleado}</td>
                                                <td className="px-4 py-2 border">${boleta.salarioBruto?.toFixed(2) ?? '0.00'}</td>
                                                <td className="px-4 py-2 border">${boleta.isss?.toFixed(2) ?? '0.00'}</td>
                                                <td className="px-4 py-2 border">${boleta.afp?.toFixed(2) ?? '0.00'}</td>
                                                <td className="px-4 py-2 border">${boleta.renta?.toFixed(2) ?? '0.00'}</td>
                                                <td className="px-4 py-2 border font-bold">${boleta.salarioNeto?.toFixed(2) ?? '0.00'}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </BoletaCard>
                        ))
                    )
                )}
            </main>
        </div>
    );
}
