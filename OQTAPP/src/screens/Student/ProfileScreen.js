/* eslint-disable react/no-unstable-nested-components */
/* eslint-disable react-native/no-inline-styles */
import {
    View,
    Text,
    StyleSheet,
    StatusBar,
    Platform,
    TouchableOpacity,
    Image
} from 'react-native'
import React, { useEffect } from 'react'
import { useAuth } from '../../context/auth'
import Icon from "react-native-vector-icons/Ionicons";
import { useNavigation } from '@react-navigation/native';
import Colors from '../../theme/Colors';
import { Image_URL } from '../../../IpConfig';

export default function ProfileScreen() {
    const navigation = useNavigation()
    const { user, logout } = useAuth();

    const RowList = [
        {
            key: 1,
            iconName: 'time',
            subtitle: 'Personal Info',
            Path: 'personal-info'
        },
        {
            key: 2,
            iconName: 'location',
            subtitle: 'Location',
            Path: 'location'
        },
        {
            key: 3,
            iconName: 'lock-closed',
            subtitle: 'Password',
            Path: 'password'
        }
    ]

    useEffect(() => {
        if (!user) {
            navigation.replace('Login');
        }
    }, [user, navigation]);
    if (!user) {
        return null;
    }

    const logoutAction = async () => {
        await logout();
        navigation.replace('Login');
    };

    const Row = ({ iconName, subtitle, Path }) => {
        return (
            <TouchableOpacity style={styles.Row}
                onPress={() => navigation.navigate(Path)}>
                <Icon name={iconName} size={20} color='#045e43dd' />
                <Text style={styles.RowText}>{subtitle}</Text>
            </TouchableOpacity>
        )
    }

    return (
        <View style={styles.safeAreaView}>
            {/* Header */}
            <View style={styles.headerContainer}>
                <TouchableOpacity
                    onPress={() => navigation.canGoBack() && navigation.goBack()}>
                    <Icon name='arrow-back' size={25} color='white' />
                </TouchableOpacity>

                <Text style={styles.headerText}>Profile</Text>

                <TouchableOpacity
                    style={styles.rightIconHeader}
                    onPress={logoutAction}>
                    <Icon name='log-out' size={25} color='white' />
                </TouchableOpacity>
            </View>
            {/* Profile Image */}
            <View style={styles.ImageContainer}>
                {!user?.profile ? (
                    <TouchableOpacity style={[styles.Image, styles.noImage]}>
                        <Text style={{ fontSize: 27, fontWeight: '300', color: '#dacd1e' }}>No Image</Text>
                        <View style={styles.cameraIcon}>
                            <Icon name='camera' size={30} color={Colors.backgroundColor} />
                        </View>
                    </TouchableOpacity>
                ) : (
                    <TouchableOpacity activeOpacity={0.8}>
                        <Image
                            source={{ uri: `${Image_URL}${user?.profile}` }}
                            style={styles.Image}
                        />
                        <TouchableOpacity style={styles.cameraIcon}>
                            <Icon name='camera' size={30} color={Colors.backgroundColor} />
                        </TouchableOpacity>
                    </TouchableOpacity>
                )}
            </View>
            <View style={styles.RowContainer}>
                {
                    RowList.map((r) => <Row
                        key={r.key.toString()}
                        iconName={r.iconName}
                        subtitle={r.subtitle}
                        Path={r.Path} />)
                }
            </View>
        </View>
    );
}


const styles = StyleSheet.create({
    safeAreaView: {
        flex: 1,
        marginTop: Platform.OS === 'android'
            ? StatusBar.currentHeight
            : 0,
        backgroundColor: '#ecececdd',
        width: '100%'
    },
    headerContainer: {
        height: 60,
        width: '100%',
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.header,
        paddingHorizontal: 10,
    },
    headerText: {
        color: 'white',
        fontSize: 22,
        marginLeft: 10
    },
    rightIconHeader: {
        position: 'absolute',
        right: 10
    },
    ImageContainer: {
        marginTop: 30,
        alignItems: 'center',
    },
    Image: {
        height: 200,
        width: 200,
        borderRadius: 100,
    },
    noImage: {
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: '#da5b5b'
    },
    cameraIcon: {
        position: 'absolute',
        bottom: 20,
        right: 10,
    },
    RowContainer: {
        flex: 1,
        marginTop: 20,
        gap: 10
    },
    Row: {
        backgroundColor: '#c6cdc4',
        padding: 10,
        justifyContent: 'flex-start',
        alignItems: 'center',
        flexDirection: 'row',
        gap: 10,
        borderColor: '#046e3fdd',
        borderWidth: 2,
        borderRadius: 5,
        margin: 5
    },
    RowText: {
        fontSize: 20
    }
});
