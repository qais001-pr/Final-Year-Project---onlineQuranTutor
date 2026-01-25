import React, { useEffect, useState } from 'react';
import {
    View,
    Image,
    TouchableOpacity,
    Text,
    StyleSheet,
    ActivityIndicator,
    Modal,
    StatusBar
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { useAuth } from '../../context/auth';

const Colors = require('../../theme/Colors').default;

export default function SplashScreen() {
    const { user } = useAuth();
    const navigation = useNavigation();
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (user === undefined) return;

        if (user) {
            const userType = user.userType;
            if (userType === 'Student' || userType === 'Child') {
                navigation.replace(`StudentDashboard`);
            }
        }
    }, [user, navigation]);

    const onNavigation = (name) => {
        setLoading(true);
        setTimeout(() => {
            navigation.navigate(name);
            setLoading(false);
        }, 800);
    };

    return (
        <View style={styles.container}>
            <StatusBar barStyle="default" />

            {/* Logo */}
            <View style={styles.logoContainer}>
                <Image
                    source={require('../../assets/images/splash.png')}
                    style={styles.logo}
                    resizeMode="contain"
                />
            </View>

            {/* Actions */}
            <View style={styles.actionsContainer}>
                <TouchableOpacity
                    style={styles.primaryButton}
                    onPress={() => onNavigation('StudentSignup')}
                >
                    <Text style={styles.primaryText}>Sign Up as Student</Text>
                </TouchableOpacity>

                <TouchableOpacity
                    style={styles.primaryButton}
                    onPress={() => onNavigation('TutorSignup')}
                >
                    <Text style={styles.primaryText}>Sign Up as Tutor</Text>
                </TouchableOpacity>

                <TouchableOpacity
                    style={styles.secondaryButton}
                    onPress={() => onNavigation('Login')}
                >
                    <Text style={styles.secondaryText}>
                        Already have an account? Login
                    </Text>
                </TouchableOpacity>
            </View>

            {/* Loading Modal */}
            <Modal transparent visible={loading}>
                <View style={styles.modalOverlay}>
                    <ActivityIndicator size="large" color="#fff" />
                </View>
            </Modal>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.backgroundColor,
    },
    logoContainer: {
        flex: 1.4,
        justifyContent: 'center',
        alignItems: 'center',
    },
    logo: {
        width: 200,
        height: 200,
    },
    actionsContainer: {
        flex: 1,
        paddingHorizontal: 24,
        justifyContent: 'center',
        gap: 16,
    },
    primaryButton: {
        backgroundColor: '#fff',
        paddingVertical: 15,
        borderRadius: 12,
        alignItems: 'center',
    },
    primaryText: {
        color: Colors.backgroundColor,
        fontSize: 16,
        fontWeight: '600',
    },
    secondaryButton: {
        marginTop: 10,
        alignItems: 'center',
    },
    secondaryText: {
        color: '#fff',
        fontSize: 15,
        fontWeight: '500',
        textDecorationLine: 'underline',
    },
    modalOverlay: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: 'rgba(0, 0, 0, 0)',
    },
});