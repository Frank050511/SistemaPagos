import { useNavigate } from 'react-router-dom';
//este es el boton que se usa para cerrar sesión, cuando se presiona se eliminan los datos del usuario y el token del localStorage,
//luego redirige al usuario a la página de inicio
const LogoutButton = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        navigate('/');
    };

    return (
        <button
            onClick={handleLogout}
            className="bg-red-500 text-white px-4 py-2 rounded"
        >
            Cerrar sesión
        </button>
    );
};

export default LogoutButton;