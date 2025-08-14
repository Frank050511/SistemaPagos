import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

const LoginForm = () => {
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        const formData = new FormData(e.target);
        const codigoEmpleado = formData.get('codigoEmpleado');
        const clave = formData.get('clave');

        try {
            const response = await fetch('https://localhost:7258/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ codigoEmpleado, clave }),
            });

            const data = await response.json();

            if (!response.ok) throw new Error(data.message || 'Error en login');
            console.log("Respuesta del servidor:", data);
            // Guarda datos y redirige
            // Verificación explícita del tipo de dato
            const isAdmin = Boolean(data.usuario.esAdmin);
            console.log("EsAdmin:", isAdmin, typeof isAdmin);

            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data.usuario));

            navigate(isAdmin ? '/admin' : '/empleado');

        } catch (err) {
            setError(err.message && 'Credenciales incorrectas');
        }
    };

    return (
        <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
            <form onSubmit={handleSubmit} className="space-y-6">
                {error && (
                    <div className="bg-red-500/10 text-red-500 p-2 rounded-md text-sm">
                        {error}
                    </div>
                )}

                <div>
                    <label htmlFor="codigoEmpleado" className="block text-sm/6 font-medium text-gray-100">
                        Codigo de Empleado:
                    </label>
                    <div className="mt-2">
                        <input
                            id="codigoEmpleado"
                            name="codigoEmpleado"
                            type="text"
                            required
                            autoComplete="username"
                            className="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-gray-500 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-500 sm:text-sm/6"
                        />
                    </div>
                </div>

                <div>
                    <div className="flex items-center justify-between">
                        <label htmlFor="clave" className="block text-sm/6 font-medium text-gray-100">
                            Clave:
                        </label>
                    </div>
                    <div className="mt-2">
                        <input
                            id="clave"
                            name="clave"
                            type="password"
                            required
                            autoComplete="current-password"
                            className="block w-full rounded-md bg-white/5 px-3 py-1.5 text-base text-white outline-1 -outline-offset-1 outline-white/10 placeholder:text-gray-500 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-500 sm:text-sm/6"
                        />
                    </div>
                </div>

                <div>
                    <button
                        type="submit"
                        className="flex w-full justify-center rounded-md bg-indigo-500 px-3 py-1.5 text-sm/6 font-semibold text-white hover:bg-indigo-400 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-500"
                    >
                        Ingresar
                    </button>
                </div>
            </form>
        </div>
    );
};

export default LoginForm;