import React, {
    createContext,
    useContext,
    useState,
    useEffect
} from "react";
import AsyncStorage from "@react-native-async-storage/async-storage";
const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);

    useEffect(() => {
        getUserData();
        console.log("Auth Context is running...");
    }, []);

    const login = async (data) => {
        try {
            await AsyncStorage.setItem("user", JSON.stringify(data));
            setUser(data);
            // console.log(data)
            await getUserData();
        } catch (error) {
            // console.log("Login error:", error);
        }
    };

    const getUserData = async () => {
        try {
            const response = await AsyncStorage.getItem("user");
            // console.log(response);
            if (response) {
                const parsedUser = JSON.parse(response);
                setUser(parsedUser);
                console.log(parsedUser);
            }
        } catch (error) {
            // console.log("Get user error:", error);
        }
    };

    const logout = async () => {
        try {
            await AsyncStorage.removeItem("user");
            setUser(null);
        } catch (error) {
            // console.log("Logout error:", error);
        }
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                login,
                logout,
                setUser
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used inside AuthProvider");
    }
    return context;
};
