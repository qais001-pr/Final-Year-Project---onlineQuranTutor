/* eslint-disable react-native/no-inline-styles */
import React, { useState } from 'react';
import {
  View,
  Image,
  TextInput,
  TouchableOpacity,
  Text,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
  StyleSheet,
  ToastAndroid,
  Modal,
  ActivityIndicator
} from 'react-native';
import { Checkbox } from 'react-native-paper';
import { Base_URL } from '../../../IpConfig';
import { useAuth } from '../../context/auth';
const Colors = require('../../theme/Colors').default;

export default function LoginScreen({ navigation }) {
  const { login } = useAuth()
  const [email, setEmail] = useState('ahmedraza12345@gmail.com');
  const [password, setPassword] = useState('123456789');
  const [loading, setloading] = useState(false);
  const [rememberme, setRememberme] = useState(false)
  const onlogin = async () => {
    setloading(false)
    if (!email || !password) {
      Platform.OS === 'android' && ToastAndroid.show('Please fill all the fields!', 4000)
      return;
    }
    try {
      const response = await fetch(`${Base_URL}auth/login?email=${email}&password=${password}`, { method: 'POST' });
      var json = await response.json();
      console.log(json)
      if (json.user) {
        const userdata = json?.user;
        if (rememberme)
          await login(userdata)
        navigation.replace(`${userdata.userType}Dashboard`)
      }
      if (response.ok) {
        Platform.OS === 'android' && ToastAndroid.show('Login successfull.', 4000)
      }
      if (!response.ok) {
        var json = await response.json();

        Platform.OS === 'android' && ToastAndroid.show(json.message || 'SignUp Failed!....', 4000)
      }
    } catch (error) {
      console.log(error)
    } finally {
      setloading(false)
    }
  }
  return (
    <View style={styles.container}>
      <KeyboardAvoidingView
        style={styles.flex}
        behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      >
        <ScrollView
          contentContainerStyle={styles.scrollContent}
          keyboardShouldPersistTaps="handled"
        >
          {/* Logo */}
          <View style={styles.logoContainer}>
            <Image
              source={require('../../assets/images/splash.png')}
              style={styles.logo}
              resizeMode="contain"
            />
          </View>

          {/* Form */}
          <View style={styles.formContainer}>
            <TextInput
              style={styles.input}
              placeholder="Email"
              placeholderTextColor="#000"
              keyboardType="email-address"
              autoCapitalize="none"
              value={email}
              onChangeText={setEmail}
            />

            <TextInput
              style={styles.input}
              placeholder="Password"
              placeholderTextColor="#000"
              secureTextEntry
              value={password}
              onChangeText={setPassword}
            />
            <View style={{ flex: 1, flexDirection: 'row', alignSelf: 'flex-start', marginLeft: 15, justifyContent: 'flex-start' }}>
              <View style={{ flex: 1, flexDirection: 'row', alignItems: 'center' }}>
                <Checkbox status={rememberme ? 'checked' : 'unchecked'}
                  onPress={() => setRememberme(!rememberme)}
                  color='#03693f'
                  uncheckedColor='#ddd'
                />
                <Text style={{ fontSize: 15, fontWeight: '400', color: '#ddd' }}>Remember Me</Text>
              </View>
            </View>
            <TouchableOpacity
              onPress={onlogin}
              style={styles.loginButton}>
              <Text style={styles.loginButtonText}>Login</Text>
            </TouchableOpacity>
            <TouchableOpacity onPress={() => navigation.navigate('ValidateEmail')}>
              <Text style={[styles.infoText, { color: 'white' }]}>Forgot your Password?</Text>
            </TouchableOpacity>

            <Text style={styles.infoText}>Don't have an account?</Text>

            <View style={styles.divider} />

            <View>
              <Text style={styles.signupText}>Sign Up</Text>
            </View>

            {/* Role Selection */}
            <View style={styles.roleContainer}>
              <TouchableOpacity style={styles.roleButton} onPress={() => navigation.navigate('StudentSignup')}>
                <Text style={styles.roleText}>Student</Text>
              </TouchableOpacity>

              <TouchableOpacity style={styles.roleButton} onPress={() => navigation.navigate('TutorSignup')}>
                <Text style={styles.roleText}>Tutor</Text>
              </TouchableOpacity>
            </View>
          </View>
        </ScrollView>
      </KeyboardAvoidingView>
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
  flex: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    justifyContent: 'center',
    padding: 15,
  },
  logoContainer: {
    alignItems: 'center',
    marginBottom: 50,
  },
  logo: {
    width: 230,
    height: 230,
  },
  formContainer: {
    alignItems: 'center',
    gap: 15,
  },
  input: {
    width: '90%',
    height: 48,
    borderWidth: 1.5,
    borderColor: Colors.border,
    borderRadius: 10,
    fontSize: 16,
    paddingHorizontal: 12,
    backgroundColor: '#fff',
  },
  loginButton: {
    width: '90%',
    height: 50,
    backgroundColor: '#094936',
    borderRadius: 10,
    justifyContent: 'center',
    alignItems: 'center',
  },
  loginButtonText: {
    fontSize: 20,
    fontWeight: '600',
    color: '#fafafa',
  },
  infoText: {
    fontSize: 18,
    color: '#000',
  },
  divider: {
    width: 300,
    height: 1,
    backgroundColor: '#ddd',
  },
  signupText: {
    fontSize: 21,
    color: '#ffffffdd',
    fontWeight: '300',
  },
  roleContainer: {
    flexDirection: 'row',
    gap: 30,
    marginTop: 10,
  },
  roleButton: {
    backgroundColor: '#094936',
    height: 50,
    paddingHorizontal: 25,
    borderRadius: 10,
    justifyContent: 'center',
    alignItems: 'center',
  },
  roleText: {
    fontSize: 20,
    fontWeight: '500',
    color: '#fafafa',
  },
});
