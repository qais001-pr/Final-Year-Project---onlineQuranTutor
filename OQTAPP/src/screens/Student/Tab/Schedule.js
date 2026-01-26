/* eslint-disable react-hooks/exhaustive-deps */
import { View, StyleSheet, Text, StatusBar, Platform, FlatList, ActivityIndicator, ScrollView } from 'react-native';
import React, { useEffect, useState } from 'react';
import Colors from '../../../theme/Colors';
import { useAuth } from '../../../context/auth';
import { Base_URL } from '../../../../IpConfig';
import { Checkbox, Modal, Portal, Provider } from 'react-native-paper';

// Fixed dimensions to ensure perfect grid alignment
const CELL_HEIGHT = 65;
const DAY_COLUMN_WIDTH = 80;
const TIME_COLUMN_WIDTH = 100;

export default function Schedule() {
    const { user } = useAuth();
    const [loading, setLoading] = useState(false);
    const [scheduleList, setScheduleList] = useState([]);
    const [timeList, setTimeList] = useState([]);

    useEffect(() => {
        if (user?.userID) fetchSchedule();
    }, [user?.userID]);

    const fetchSchedule = async () => {
        try {
            setLoading(true);
            const response = await fetch(`${Base_URL}Slots/GetSlotsWithDay?userid=${user.userID}`);
            if (response.ok) {
                const data = await response.json();
                if (data.length > 0) {
                    setTimeList(data[0].Slots);
                    setScheduleList(data);
                }
            }
        } catch (error) {
            console.error("Fetch Error:", error);
        } finally {
            setLoading(false);
        }
    };

    const getDayLabel = (day) => {
        const days = { Monday: 'Mon', Tuesday: 'Tue', Wednesday: 'Wed', Thursday: 'Thu', Friday: 'Fri', Saturday: 'Sat', Sunday: 'Sun' };
        return days[day] || day;
    };

    // 1. Time Slot Renderer (Left Column)
    const renderTimeItem = ({ item, index }) => (
        <View style={styles.timeCell}>
            <Text style={styles.timeText}>{index + 1}</Text>
            <Text style={styles.timeText}>{item?.StartTime}</Text>
            <Text style={styles.timeTextSub}>{item?.EndTime}</Text>
        </View>
    );

    // 2. Checkbox Renderer (Inside Day Column)
    const renderCheckboxCell = ({ item, index }) => (
        <View style={styles.checkboxCell}>
            <Text style={styles.timeText}>{index + 1}</Text>
            <Checkbox
                status={item.Status.toString() === 'booked' ? 'checked' : 'unchecked'}
                onPress={() => console.log("Selected Slot:", item)}
                color={Colors.header || '#6200ee'}
            />
        </View>
    );

    // 3. Day Column Renderer (Horizontal List Item)
    const renderDayColumn = ({ item }) => (
        <View style={styles.dayColumn}>
            <View style={styles.columnHeader}>
                <Text style={styles.dayHeaderText}>{getDayLabel(item?.DayName)}</Text>
            </View>
            <FlatList
                data={item?.Slots}
                keyExtractor={(slot) => slot.SlotID.toString()}
                renderItem={renderCheckboxCell}
            />
        </View>
    );

    return (
        <Provider>
            <View style={styles.safeAreaView}>
                <StatusBar barStyle={'light-content'} backgroundColor={Colors.header || '#6200ee'} />

                <View style={styles.headerBar}>
                    <Text style={styles.headerTitle}>Schedule</Text>
                </View>

                <View style={styles.gridContainer}>
                    {/* LEFT SIDE: TIME LABELS */}
                    <View style={{ width: TIME_COLUMN_WIDTH }}>
                        <View style={styles.columnHeader}>
                            <Text style={styles.headerLabelText}>Time</Text>
                        </View>
                        <FlatList
                            data={timeList}
                            keyExtractor={(item) => item.SlotID.toString()}
                            renderItem={renderTimeItem}
                            showsVerticalScrollIndicator={false}
                        />
                    </View>

                    {/* RIGHT SIDE: SCROLLABLE CHECKBOX GRID */}
                    <ScrollView horizontal showsHorizontalScrollIndicator={true}>
                        <FlatList
                            data={scheduleList}
                            keyExtractor={(item) => item.DayID.toString()}
                            renderItem={renderDayColumn}
                            horizontal={true}
                            showsVerticalScrollIndicator={false}
                        />
                    </ScrollView>
                </View>

                <Portal>
                    <Modal visible={loading} dismissable={false}>
                        <ActivityIndicator size="large" color="white" />
                    </Modal>
                </Portal>
            </View>
        </Provider>
    );
}

const styles = StyleSheet.create({
    safeAreaView: {
        flex: 1,
        backgroundColor: '#f4f4f4',
        marginTop: Platform.OS === 'android' ? StatusBar.currentHeight : 0,
    },
    headerBar: {
        height: 60,
        justifyContent: 'center',
        paddingHorizontal: 20,
        backgroundColor: Colors.header || '#6200ee',
        elevation: 3,
    },
    headerTitle: {
        color: '#fff',
        fontSize: 20,
        fontWeight: 'bold',
    },
    gridContainer: {
        flex: 1,
        flexDirection: 'row',
        backgroundColor: '#fff',
    },
    /* Column Shared Styles */
    columnHeader: {
        height: 50,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: '#f9f9f9',
        borderBottomWidth: 1,
        borderBottomColor: '#e0e0e0',
        borderRightWidth: 1,
        borderRightColor: '#e0e0e0',
    },
    headerLabelText: {
        fontWeight: 'bold',
        color: '#888',
        fontSize: 12,
        textTransform: 'uppercase',
    },
    dayHeaderText: {
        fontWeight: 'bold',
        color: '#333',
        fontSize: 14,
    },
    /* Time Column */
    timeCell: {
        height: CELL_HEIGHT,
        width: TIME_COLUMN_WIDTH,
        justifyContent: 'center',
        alignItems: 'center',
        borderBottomWidth: 1,
        borderBottomColor: '#f0f0f0',
        borderRightWidth: 1,
        borderRightColor: '#e0e0e0',
    },
    timeText: {
        fontSize: 13,
        fontWeight: '600',
        color: '#444',
    },
    timeTextSub: {
        fontSize: 11,
        color: '#999',
    },
    /* Day Columns */
    dayColumn: {
        width: DAY_COLUMN_WIDTH,
    },
    checkboxCell: {
        height: CELL_HEIGHT,
        justifyContent: 'center',
        alignItems: 'center',
        borderBottomWidth: 1,
        borderBottomColor: '#f0f0f0',
        borderRightWidth: 1,
        borderRightColor: '#f0f0f0',
    }
});