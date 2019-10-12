from math import floor
"""A helper module to manage time"""
def convert_to_flat(time12 : str) -> int:
    """Converts 12 hour time to flat integer based time considering 12am as 0"""
    time12 = str(time12).lower()
    if 'am' in time12:
        time12 = time12.replace('am','').strip()
        hour, minute = time12.split(':')
        hour = int(hour)
        minute = int(minute)
        return (hour * 60 + minute) % 720
    elif 'pm' in time12:
        time12 = time12.replace('pm','').strip()
        hour, minute = time12.split(':')
        hour = int(hour)
        minute = int(minute)
        return 720 + (hour * 60 + minute) % 720
    else:
        return -1

def convert_to_time12(flat : int) -> str:
    """Converts flat time back to 12 hour time format"""
    flat = int(flat) % 1440
    mer = 'am'
    if flat >= 720:
        mer = 'pm'
        flat -= 720
    hour = int(floor(flat / 60))
    minute = flat % 60
    if hour == 0:
        hour = 12
    return str(hour).rjust(2, '0') + ':' + str(minute).rjust(2, '0') + mer   

def get_flat_difference(time_low : str, time_high : str) -> int:
    """Returns the flat differences between 2 12 hour time formats"""
    time_low = str(time_low)
    time_high = str(time_high)
    f_1 = convert_to_flat(time_low)
    f_2 = convert_to_flat(time_high)
    return (f_2 - f_1) % 1440

def add_flat_to_time12(time12 : str, flat : int) -> str:
    """Adds a flat time to 12 hour time format and returns 12 hour time"""
    f = convert_to_flat(time12)
    f = f + flat
    return convert_to_time12(f)