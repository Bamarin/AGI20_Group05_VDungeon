#!/usr/bin/env python3
import cv2
import numpy as np
import dlib
import socket

UDP_IP = "127.0.0.1"
UDP_PORT = 5065
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
cap = cv2.VideoCapture(0)

width = cap.get(cv2.CAP_PROP_FRAME_WIDTH)
height = cap.get(cv2.CAP_PROP_FRAME_HEIGHT)

# Camera internals
focal_length = width   # image width
center = (width/2, height/2)
camera_matrix = np.array(
                         [[focal_length, 0, center[0]],
                         [0, focal_length, center[1]],
                         [0, 0, 1]], dtype = "double"
                         )

print("Camera Matrix :\n {0}".format(camera_matrix))  

# convert 68 (x, y)-coordinates to a NumPy array
def landmarks_to_np(shape, dtype="int"):
    # initialize the list of (x, y)-coordinates
    coords = np.zeros((68, 2), dtype=dtype)  
    # loop over the 68 facial landmarks and convert them to a 2-tuple of (x, y)-coordinates
    for i in range(0, 68):
        coords[i] = (shape.part(i).x, shape.part(i).y)
    return coords

def get_facial_parameter(landmarks):
    d00 =np.linalg.norm(landmarks[27]-landmarks[8]) # Length of face (eyebrow to chin)
    d11 =np.linalg.norm(landmarks[0]-landmarks[16]) # width of face
    d_reference = (d00+d11)/2
    # Left eye
    d1 =  np.linalg.norm(landmarks[37]-landmarks[41])
    d2 =  np.linalg.norm(landmarks[38]-landmarks[40])
    # Right eye
    d3 =  np.linalg.norm(landmarks[43]-landmarks[47])
    d4 =  np.linalg.norm(landmarks[44]-landmarks[46])
    # Mouth width
    d5 = np.linalg.norm(landmarks[51]-landmarks[57])
    # Mouth length
    d6 = np.linalg.norm(landmarks[60]-landmarks[64])
    
    leftEyeWid = ((d1+d2)/(2*d_reference) - 0.02)*6
    rightEyewid = ((d3+d4)/(2*d_reference) -0.02)*6
    mouthWid = (d5/d_reference - 0.13)*1.27+0.02
    mouthLen = d6/d_reference

    return leftEyeWid, rightEyewid, mouthWid, mouthLen

# Head Pose Estimation function: get rotation vector and translation vector       
def head_pose_estimate(image, landmarks):
    
    #2D image points. 
    image_points = np.array([   landmarks[30],     # Nose tip
                                landmarks[8],      # Chin
                                landmarks[36],     # Left eye left corner
                                landmarks[45],     # Right eye right corne
                                landmarks[48],     # Left Mouth corner
                                landmarks[54]      # Right mouth corner
                            ], dtype="double")
    # 3D model points.
    model_points = np.array([   (0.0, 0.0, 0.0),             # Nose tip
                                (0.0, -330.0, -65.0),        # Chin
                                (-225.0, 170.0, -135.0),     # Left eye left corner
                                (225.0, 170.0, -135.0),      # Right eye right corne
                                (-150.0, -150.0, -125.0),    # Left Mouth corner
                                (150.0, -150.0, -125.0)      # Right mouth corner
                         
                            ])

    # Input vector of distortion coefficients, Assuming no lens distortion
    dist_coeffs = np.zeros((4,1))  
        
    (success, rotation_vector, translation_vector) = cv2.solvePnP(model_points, image_points, camera_matrix, dist_coeffs, flags=cv2.SOLVEPNP_ITERATIVE)
     
    print("Rotation Vector:\n {0}".format(rotation_vector))
    print("Translation Vector:\n {0}".format(translation_vector))
     
    # visualize the rotation by drawing a line from nose to a 3D point (0, 0, 1000.0)
    (nose_end_point2D, jacobian) = cv2.projectPoints(np.array([(0.0, 0.0, 1000.0)]), rotation_vector, translation_vector, camera_matrix, dist_coeffs)
     
    for p in image_points:
        cv2.circle(image, (int(p[0]), int(p[1])), 3, (0,0,255), -1)  
     
    p1 = ( int(image_points[0][0]), int(image_points[0][1]))
    p2 = ( int(nose_end_point2D[0][0][0]), int(nose_end_point2D[0][0][1]))
     
    cv2.line(image, p1, p2, (255,0,0), 2)
    return success, rotation_vector, translation_vector, camera_matrix, dist_coeffs

# initialize dlib's pre-trained face detector and load the facial landmark predictor
detector = dlib.get_frontal_face_detector()
predictor = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat")


while True:
    
    ret, frame = cap.read()
    # convert frame into grayscale
    gray_image = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY) 
    faces = detector(gray_image, 0)
    
    for face in faces:    
        # determine the facial landmarks for the face region
        shape = predictor(gray_image, face)
        # convert the facial landmark (x, y)-coordinates to a NumPy array (68*2)
        landmarks = landmarks_to_np(shape)

        # Show facial parameter
        leftEyeWid, rightEyewid, mouthWid,mouthLen =get_facial_parameter(landmarks)
        print('leftEyeWid:{}, rightEyewid:{}, mouthWid:{}, mouthLen:{}'.format(leftEyeWid, rightEyewid, mouthWid, mouthLen))
        
        # Show head pose
        ret, rotation_vector, translation_vector, camera_matrix, dist_coeffs = head_pose_estimate(frame, landmarks)
        face_data = str(translation_vector[0,0])+':'+str(translation_vector[1,0])+':'+str(translation_vector[2,0])+':'+str(rotation_vector[0,0])+':'+str(rotation_vector[1,0])+':'+str(rotation_vector[2,0])+':'+str(w)+':'+str(x)+':'+str(y)+':'+str(z)+':'+str(leftEyeWid)+':'+str(rightEyewid)+':'+str(mouthWid)+':'+str(mouthLen)
        sock.sendto(face_data.encode() , (UDP_IP, UDP_PORT))        
        # loop over the (x, y)-coordinates for the facial landmarks and draw them on the image
        for (x, y) in landmarks:
            cv2.circle(frame, (x, y), 2, (255, 0, 0), -1) 
            
    cv2.imshow('Video', frame)

    key = cv2.waitKey(1)
    if key == 27:
        break
 
cap.release()
cv2.destroyAllWindows()
