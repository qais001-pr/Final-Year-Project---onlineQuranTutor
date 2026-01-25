/* eslint-disable react/self-closing-comp */
/* eslint-disable react-native/no-inline-styles */
import { View, Text, Platform, StatusBar, Image, StyleSheet, TouchableOpacity } from 'react-native'
import React, { useEffect } from 'react'
import { useAuth } from '../../../context/auth'
import { useNavigation } from '@react-navigation/native'
import Colors from '../../../theme/Colors'
import { Image_URL } from '../../../../IpConfig'
export default function StudentHomeDashboard() {
    const { user } = useAuth()
    const navigation = useNavigation()
    useEffect(() => {
        if (!user) {
            navigation.replace('Splash')
        }
    }, [user, navigation])


    return (
        <View style={{
            flex: 1, marginTop: Platform.OS === 'android' && StatusBar.currentHeight,
        }}>
            <StatusBar barStyle={'dark-content'} />
            <View style={style.headerContainer}>
                <Text style={style.headerText}>{user?.name || ''}</Text>
                {
                    user?.profile === null ? (
                        <View style={style.headerImage}>
                            <Text style={{ textAlign: 'center' }}>No Image</Text>
                        </View>
                    ) : (
                        <View style={style.headerImage}>
                            <TouchableOpacity

                                onPress={() => navigation.navigate('StudentProfile')}>
                                <Image source={{ uri: `${Image_URL}${user?.profile}` }} height={40} width={40} borderRadius={100} />
                            </TouchableOpacity>
                        </View>
                    )
                }
            </View>
            <View style={{ flex: 1 }}></View>
        </View >
    )
}

const style = StyleSheet.create({
    headerContainer: {
        height: 70,
        backgroundColor: Colors.header,
        flexDirection: 'row',
        justifyContent: 'space-between',
        padding: 10,
        alignItems: 'center',
    },
    headerText: {
        color: 'white',
        justifyContent: 'flex-start',
        fontSize: 22,
    },
    headerImage: {
        justifyContent: 'center',
        backgroundColor: Colors.secondary,
        width: 50,
        height: 50,
        borderRadius: 100,
        margin: 10,
        alignItems: 'center'
    }

})